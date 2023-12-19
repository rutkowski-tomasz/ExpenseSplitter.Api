using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Expenses.GetExpense;

internal sealed class GetExpenseQueryHandler : IQueryHandler<GetExpenseQuery, GetExpenseQueryResult>
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

    public async Task<Result<GetExpenseQueryResult>> Handle(GetExpenseQuery request, CancellationToken cancellationToken)
    {
        var expenseId = new ExpenseId(request.ExpenseId);
        var expense = await expenseRepository.GetById(expenseId, cancellationToken);
        if (expense is null)
        {
            return Result.Failure<GetExpenseQueryResult>(ExpenseErrors.NotFound);
        }

        if (!await settlementUserRepository.CanUserAccessSettlement(expense.SettlementId, cancellationToken))
        {
            return Result.Failure<GetExpenseQueryResult>(SettlementErrors.Forbidden);
        }

        var allocations = await allocationRepository.GetAllByExpenseId(expenseId, cancellationToken);

        var response = new GetExpenseQueryResult(
            expense.Id.Value,
            expense.Title,
            expense.PayingParticipantId.Value,
            expense.PaymentDate,
            expense.Amount.Value,
            allocations.Select(x => new GetExpenseQueryResultAllocation(
                x.Id.Value,
                x.ParticipantId.Value,
                x.Amount.Value
            ))
        );

        return Result.Success(response);
    }
}
