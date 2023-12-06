using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Application.Expenses.GetExpensesForSettlement;

internal sealed class GetExpensesForSettlementQueryHandler : IQueryHandler<GetExpensesForSettlementQuery, GetExpensesForSettlementQueryResult>
{
    private readonly IExpenseRepository expenseRepository;

    public GetExpensesForSettlementQueryHandler(IExpenseRepository expenseRepository)
    {
        this.expenseRepository = expenseRepository;
    }

    public async Task<Result<GetExpensesForSettlementQueryResult>> Handle(GetExpensesForSettlementQuery request, CancellationToken cancellationToken)
    {
        var expenses = await expenseRepository
            .GetAllWithSettlementId(new SettlementId(request.SettlementId), cancellationToken);

        var resultExpenses = expenses.Select(x => new GetExpensesForSettlementQueryResultExpense(
            x.Id,
            x.Title,
            x.Amount.Value
        ));
        var result = new GetExpensesForSettlementQueryResult(resultExpenses);
        
        return Result.Success(result);
    }
}
