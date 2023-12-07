using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.ExpenseAllocations;
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
    private readonly IExpenseAllocationRepository expenseAllocationRepository;
    private readonly IUnitOfWork unitOfWork;

    public UpdateExpenseCommandHandler(
        IExpenseRepository expenseRepository,
        ISettlementUserRepository settlementUserRepository,
        IExpenseAllocationRepository expenseAllocationRepository,
        IUnitOfWork unitOfWork
    )
    {
        this.expenseRepository = expenseRepository;
        this.settlementUserRepository = settlementUserRepository;
        this.expenseAllocationRepository = expenseAllocationRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        var expenseId = new ExpenseId(request.Id);
        var expense = await expenseRepository.GetByIdAsync(expenseId, cancellationToken);
        if (expense is null)
        {
            return Result.Failure(ExpenseErrors.NotFound);
        }

        if (!await settlementUserRepository.CanUserAccessSettlement(expense.SettlementId, cancellationToken))
        {
            return Result.Failure(SettlementErrors.Forbidden);
        }

        UpdateExpense(expense, request);

        if (request.Allocations is not null)
        {
            await UpdateAllocations(request, expenseId, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    private async Task UpdateAllocations(UpdateExpenseCommand request, ExpenseId expenseId, CancellationToken cancellationToken)
    {
        var allocations = await expenseAllocationRepository.GetAllWithExpenseId(expenseId, cancellationToken);
        
        var allocationsToRemove = allocations
            .Where(a => !request.Allocations.Any(ra => ra.Id.HasValue && new ExpenseAllocationId(ra.Id.Value) == a.Id))
            .ToList();
        
        foreach (var allocation in allocationsToRemove)
        {
            expenseAllocationRepository.Remove(allocation);
        }

        foreach (var requestAllocation in request.Allocations)
        {
            if (requestAllocation.Id is null)
            {
                AddNewAllocation(requestAllocation, expenseId);
                continue;
            }

            var expenseAllocationId = new ExpenseAllocationId(requestAllocation.Id.Value);
            var existingAllocation = await expenseAllocationRepository.GetWithId(expenseAllocationId, cancellationToken);

            if (existingAllocation is null)
            {
                continue;
            }

            UpdateExistingAllocation(requestAllocation, existingAllocation);
        }
    }

    private void AddNewAllocation(UpdateExpenseCommandAllocation requestAllocation, ExpenseId expenseId)
    {
        var newAllocation = ExpenseAllocation.Create(
            new Amount(requestAllocation.Value!.Value),
            expenseId,
            new ParticipantId(requestAllocation.ParticipantId!.Value)
        );
        expenseAllocationRepository.Add(newAllocation);
    }

    private void UpdateExistingAllocation(UpdateExpenseCommandAllocation requestAllocation, ExpenseAllocation existingAllocation)
    {
        if (requestAllocation.Value is not null)
        {
            existingAllocation.SetAmount(new Amount(requestAllocation.Value.Value));
        }

        if (requestAllocation.ParticipantId is not null)
        {
            var participantId = new ParticipantId(requestAllocation.ParticipantId.Value);
            existingAllocation.SetParticipantId(participantId);
        }
    }

    private void UpdateExpense(Expense expense, UpdateExpenseCommand request)
    {
        if (request.Title is not null)
        {
            expense.SetTitle(request.Title);
        }

        if (request.Amount is not null)
        {
            expense.SetAmount(new Amount(request.Amount.Value));
        }

        if (request.Date is not null)
        {
            expense.SetPaymentDate(request.Date.Value);
        }

        if (request.PayingParticipantId is not null)
        {
            var participantId = new ParticipantId(request.PayingParticipantId.Value);
            expense.SetPayingParticipantId(participantId);
        }
    }
}
