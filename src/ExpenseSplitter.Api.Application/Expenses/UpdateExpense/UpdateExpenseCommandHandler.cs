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

        var allocations = await expenseAllocationRepository.GetAllWithExpenseId(expenseId, cancellationToken);
        RemoveNonExistingAllocations(allocations, request);
        CreateNewAllocations(request, expenseId);
        UpdateExistingAllocations(allocations, request);

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    private void UpdateExpense(Expense expense, UpdateExpenseCommand request)
    {
        expense.SetTitle(request.Title);
        expense.SetAmount(new Amount(request.Amount));
        expense.SetPaymentDate(request.Date);
        expense.SetPayingParticipantId(new ParticipantId(request.PayingParticipantId));
    }

    private void RemoveNonExistingAllocations(IEnumerable<ExpenseAllocation> allocations, UpdateExpenseCommand updateCommand)
    {
        var allocationsToRemove = allocations
            .Where(x => !updateCommand.Allocations.Any(y => y.Id.HasValue && new ExpenseAllocationId(y.Id.Value) == x.Id));

        foreach (var allocationToRemove in allocationsToRemove)
        {
            expenseAllocationRepository.Remove(allocationToRemove);
        }
    }

    private void CreateNewAllocations(UpdateExpenseCommand updateCommand, ExpenseId expenseId)
    {
        var newAllocations = updateCommand
            .Allocations
            .Where(x => !x.Id.HasValue)
            .Select(x => ExpenseAllocation.Create(
                new Amount(x.Value),
                expenseId,
                new ParticipantId(x.ParticipantId)
            ));
        
        foreach (var newAllocation in newAllocations)
        {
            expenseAllocationRepository.Add(newAllocation);
        }
    }

    private void UpdateExistingAllocations(IEnumerable<ExpenseAllocation> allocations, UpdateExpenseCommand updateCommand)
    {
        var updates = updateCommand
            .Allocations
            .Where(x => x.Id.HasValue)
            .Select(x => new {
                UpdateModel = x,
                Entity = allocations.Single(y => y.Id == new ExpenseAllocationId(x.Id!.Value))
            });

        foreach (var update in updates)
        {
            update.Entity.SetAmount(new Amount(update.UpdateModel.Value));
            update.Entity.SetParticipantId(new ParticipantId(update.UpdateModel.ParticipantId));
        }
    }
}
