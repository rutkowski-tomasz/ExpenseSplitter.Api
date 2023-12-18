using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
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
    private readonly IUnitOfWork unitOfWork;

    public UpdateExpenseCommandHandler(
        IExpenseRepository expenseRepository,
        ISettlementUserRepository settlementUserRepository,
        IAllocationRepository expenseAllocationRepository,
        IUnitOfWork unitOfWork
    )
    {
        this.expenseRepository = expenseRepository;
        this.settlementUserRepository = settlementUserRepository;
        this.allocationRepository = expenseAllocationRepository;
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

        var updateResult = UpdateExpense(expense, request);
        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        var allocations = (await allocationRepository
            .GetAllByExpenseId(expenseId, cancellationToken))
            .ToList();
        
        RemoveNonExistingAllocations(allocations, request);
        CreateNewAllocations(request, expenseId);
        UpdateExistingAllocations(allocations, request);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private Result UpdateExpense(Expense expense, UpdateExpenseCommand request)
    {
        var setTitleResult = expense.SetTitle(request.Title);
        if (setTitleResult.IsFailure)
        {
            return setTitleResult;
        }

        var totalAmount = request.Allocations.Sum(x => x.Value);
        var setAmountResult = expense.SetAmount(new Amount(totalAmount));
        if (setAmountResult.IsFailure)
        {
            return setAmountResult;
        }

        expense.SetPaymentDate(request.Date);
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

    private void CreateNewAllocations(UpdateExpenseCommand updateCommand, ExpenseId expenseId)
    {
        var newAllocations = updateCommand
            .Allocations
            .Where(x => !x.Id.HasValue)
            .Select(x => Allocation.Create(
                new Amount(x.Value),
                expenseId,
                new ParticipantId(x.ParticipantId)
            ));
        
        foreach (var newAllocation in newAllocations)
        {
            allocationRepository.Add(newAllocation);
        }
    }

    private void UpdateExistingAllocations(IEnumerable<Allocation> allocations, UpdateExpenseCommand updateCommand)
    {
        var updates = updateCommand
            .Allocations
            .Where(x => x.Id.HasValue)
            .Select(x => new {
                UpdateModel = x,
                Entity = allocations.Single(y => y.Id == new AllocationId(x.Id!.Value))
            });

        foreach (var update in updates)
        {
            update.Entity.SetAmount(new Amount(update.UpdateModel.Value));
            update.Entity.SetParticipantId(new ParticipantId(update.UpdateModel.ParticipantId));
        }
    }
}
