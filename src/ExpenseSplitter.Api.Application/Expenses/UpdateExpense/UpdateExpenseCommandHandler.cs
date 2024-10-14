using ExpenseSplitter.Api.Application.Abstractions.Clock;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Application.Exceptions;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;
using ExpenseSplitter.Api.Domain.Shared;

namespace ExpenseSplitter.Api.Application.Expenses.UpdateExpense;

public class UpdateExpenseCommandHandler : ICommandHandler<UpdateExpenseCommand>
{
    private readonly IExpenseRepository expenseRepository;
    private readonly ISettlementUserRepository settlementUserRepository;
    private readonly IAllocationRepository allocationRepository;
    private readonly ISettlementRepository settlementRepository;
    private readonly IDateTimeProvider dateTimeProvider;
    private readonly IUnitOfWork unitOfWork;

    public UpdateExpenseCommandHandler(
        IExpenseRepository expenseRepository,
        ISettlementUserRepository settlementUserRepository,
        IAllocationRepository allocationRepository,
        ISettlementRepository settlementRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork
    )
    {
        this.expenseRepository = expenseRepository;
        this.settlementUserRepository = settlementUserRepository;
        this.allocationRepository = allocationRepository;
        this.settlementRepository = settlementRepository;
        this.dateTimeProvider = dateTimeProvider;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        var expenseId = new ExpenseId(request.Id);
        var expense = await expenseRepository.GetById(expenseId, cancellationToken);
        if (expense is null)
        {
            return Result.Failure(ExpenseErrors.NotFound);
        }

        if (!await settlementUserRepository.CanUserAccessSettlement(expense.SettlementId, cancellationToken))
        {
            return Result.Failure(SettlementErrors.Forbidden);
        }

        var settlement = await settlementRepository.GetById(expense.SettlementId, cancellationToken);
        settlement!.SetUpdatedOnUtc(dateTimeProvider.UtcNow);

        var updateResult = UpdateExpense(expense, request);
        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        var allocations = (await allocationRepository
            .GetAllByExpenseId(expenseId, cancellationToken))
            .ToList();
        
        RemoveNonExistingAllocations(allocations, request);
        var createResult = CreateNewAllocations(request, expenseId);
        if (createResult.IsFailure)
        {
            return createResult;
        }

        var updateAllocationsResult = UpdateExistingAllocations(allocations, request);
        if (updateAllocationsResult.IsFailure)
        {
            return updateAllocationsResult;
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyException)
        {
            return Result.Failure(ConcurrencyException.ConcurrencyError);
        }

        return Result.Success();
    }

    private static Result UpdateExpense(Expense expense, UpdateExpenseCommand request)
    {
        var setTitleResult = expense.SetTitle(request.Title);
        if (setTitleResult.IsFailure)
        {
            return setTitleResult;
        }

        var totalAmountResult = Amount.Create(request.Allocations.Sum(x => x.Value));
        if (totalAmountResult.IsFailure)
        {
            return totalAmountResult;
        }

        expense.SetAmount(totalAmountResult.Value);
        expense.SetPaymentDate(request.PaymentDate);
        expense.SetPayingParticipantId(new ParticipantId(request.PayingParticipantId));
        return Result.Success();
    }

    private void RemoveNonExistingAllocations(IEnumerable<Allocation> allocations, UpdateExpenseCommand updateCommand)
    {
        var allocationsToRemove = allocations
            .Where(x => !updateCommand.Allocations.Any(y => y.Id.HasValue && new AllocationId(y.Id.Value) == x.Id));

        foreach (var allocationToRemove in allocationsToRemove)
        {
            allocationRepository.Remove(allocationToRemove);
        }
    }

    private Result CreateNewAllocations(UpdateExpenseCommand updateCommand, ExpenseId expenseId)
    {
        var newAllocations = updateCommand
            .Allocations
            .Where(x => !x.Id.HasValue);

        foreach (var newAllocation in newAllocations)
        {
            var amountResult = Amount.Create(newAllocation.Value);
            if (amountResult.IsFailure)
            {
                return amountResult;
            }

            var allocation = Allocation.Create(
                amountResult.Value,
                expenseId,
                new ParticipantId(newAllocation.ParticipantId)
            );

            allocationRepository.Add(allocation);
        }

        return Result.Success();
    }

    private static Result UpdateExistingAllocations(IEnumerable<Allocation> allocations, UpdateExpenseCommand updateCommand)
    {
        var updates = updateCommand
            .Allocations
            .Where(x => x.Id.HasValue)
            .Select(x => new {
                UpdateModel = x,
                Entity = allocations.SingleOrDefault(y => y.Id == new AllocationId(x.Id!.Value))
            })
            .Where(x => x.Entity is not null);

        foreach (var update in updates)
        {
            var amountResult = Amount.Create(update.UpdateModel.Value);
            if (amountResult.IsFailure)
            {
                return amountResult;
            }

            update.Entity!.SetAmount(amountResult.Value);
            update.Entity.SetParticipantId(new ParticipantId(update.UpdateModel.ParticipantId));
        }

        return Result.Success();
    }
}
