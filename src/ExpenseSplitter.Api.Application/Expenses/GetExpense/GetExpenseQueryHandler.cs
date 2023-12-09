﻿using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Settlements.GetExpense;

internal sealed class GetExpenseQueryHandler : IQueryHandler<GetExpenseQuery, GetExpenseResponse>
{
    private readonly IExpenseRepository expenseRepository;
    private readonly IAllocationRepository allocationRepository;
    private readonly ISettlementUserRepository settlementUserRepository;

    public GetExpenseQueryHandler(
        IExpenseRepository expenseRepository,
        IAllocationRepository allocationRepository,
        ISettlementUserRepository settlementUserRepository
    )
    {
        this.expenseRepository = expenseRepository;
        this.allocationRepository = allocationRepository;
        this.settlementUserRepository = settlementUserRepository;
    }

    public async Task<Result<GetExpenseResponse>> Handle(GetExpenseQuery request, CancellationToken cancellationToken)
    {
        var expenseId = new ExpenseId(request.ExpenseId);
        var expense = await expenseRepository.GetByIdAsync(expenseId, cancellationToken);
        if (expense is null)
        {
            return Result.Failure<GetExpenseResponse>(ExpenseErrors.NotFound);
        }

        if (!await settlementUserRepository.CanUserAccessSettlement(expense.SettlementId, cancellationToken))
        {
            return Result.Failure<GetExpenseResponse>(SettlementErrors.Forbidden);
        }

        var allocations = await allocationRepository.GetAllWithExpenseId(expenseId, cancellationToken);

        var response = new GetExpenseResponse(
            expense.Id.Value,
            expense.Title,
            expense.PayingParticipantId.Value,
            expense.PaymentDate,
            expense.Amount.Value,
            allocations.Select(x => new GetExpenseResponseAllocation(
                x.Id.Value,
                x.ParticipantId.Value,
                x.Amount.Value
            ))
        );

        return Result.Success(response);
    }
}
