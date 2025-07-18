using ExpenseSplitter.Api.Application.Abstractions.Clock;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Application.Exceptions;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Common;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Expenses.UpdateExpense;

public class UpdateExpenseCommandHandler(
    IExpenseRepository expenseRepository,
    ISettlementUserRepository settlementUserRepository,
    ISettlementRepository settlementRepository,
    IDateTimeProvider timeProvider,
    IUnitOfWork unitOfWork
) : ICommandHandler<UpdateExpenseCommand>
{
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
        settlement!.SetUpdatedOnUtc(timeProvider.UtcNow);

        var updateResult = UpdateExpense(expense, request);
        if (updateResult.IsFailure)
        {
            return updateResult;
        }
        
        var removeResult = RemoveNonExistingAllocations(expense, request);
        if (removeResult.IsFailure)
        {
            return removeResult;
        }
        
        var createResult = CreateNewAllocations(expense, request);
        if (createResult.IsFailure)
        {
            return createResult;
        }

        var updateAllocationsResult = UpdateExistingAllocations(expense, request);
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
            return Result.Failure(ConcurrencyException.ConcurrencyAppError);
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

        expense.SetPaymentDate(request.PaymentDate);
        expense.SetPayingParticipantId(new ParticipantId(request.PayingParticipantId));
        return Result.Success();
    }

    private static Result RemoveNonExistingAllocations(Expense expense, UpdateExpenseCommand updateCommand)
    {
        var allocationIds = updateCommand.Allocations
            .Where(x => x.Id.HasValue)
            .Select(x => new AllocationId(x.Id!.Value));

        var allocationsToRemove = expense.Allocations
            .Where(x => !allocationIds.Contains(x.Id))
            .ToList();

        foreach (var allocationToRemove in allocationsToRemove)
        {
            var removeAllocationResult = expense.RemoveAllocation(allocationToRemove.Id);
            if (removeAllocationResult.IsFailure)
            {
                return removeAllocationResult;
            }
        }

        return Result.Success();
    }

    private static Result CreateNewAllocations(Expense expense, UpdateExpenseCommand updateCommand)
    {
        var newAllocations = updateCommand.Allocations
            .Where(x => !x.Id.HasValue);

        foreach (var newAllocation in newAllocations)
        {
            var amountResult = Amount.Create(newAllocation.Value);
            if (amountResult.IsFailure)
            {
                return amountResult;
            }

            var addAllocationResult = expense
                .AddAllocation(amountResult.Value, new ParticipantId(newAllocation.ParticipantId));
            if (addAllocationResult.IsFailure)
            {
                return addAllocationResult;
            }
        }

        return Result.Success();
    }

    private static Result UpdateExistingAllocations(Expense expense, UpdateExpenseCommand updateCommand)
    {
        var updateModels = updateCommand.Allocations
            .Where(x => x.Id.HasValue);

        foreach (var updateModel in updateModels)
        {
            var allocationId = new AllocationId(updateModel.Id!.Value);
            var allocation = expense.Allocations.FirstOrDefault(y => y.Id == allocationId);
            if (allocation is null)
            {
                return ExpenseErrors.AllocationNotFound;
            }

            var amountResult = Amount.Create(updateModel.Value);
            if (amountResult.IsFailure)
            {
                return amountResult;
            }

            var participantId = new ParticipantId(updateModel.ParticipantId);

            var updateResult = expense.UpdateAllocation(allocationId, amountResult.Value, participantId);
            if (updateResult.IsFailure)
            {
                return updateResult;
            }
        }

        return Result.Success();
    }
}
