﻿using ExpenseSplitter.Api.Application.Abstractions.Clock;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Expenses.DeleteExpense;

public class DeleteExpenseCommandHandler(
        ISettlementUserRepository settlementUserRepository,
        IExpenseRepository expenseRepository,
        ISettlementRepository settlementRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork
) : ICommandHandler<DeleteExpenseCommand>
{
    public async Task<Result> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
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

        expenseRepository.Remove(expense);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
