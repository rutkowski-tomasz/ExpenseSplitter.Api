using ExpenseSplitter.Api.Application.Expenses.GetExpensesForSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public sealed record GetExpensesForSettlementResponse(
    IEnumerable<GetExpensesForSettlementResponseExpense> Expenses
);

public sealed record GetExpensesForSettlementResponseExpense(
    Guid Id,
    string Title,
    decimal Amount,
    Guid PayingParticipantId,
    DateOnly PaymentDate
);


public class SettlementGetExpensesEndpoint : IEndpoint,
    IMapper<Guid, GetExpensesForSettlementQuery>,
    IMapper<GetExpensesForSettlementQueryResult, GetExpensesForSettlementResponse>
{
    public GetExpensesForSettlementQuery Map(Guid source)
    {
        return new(source);
    }

    public GetExpensesForSettlementResponse Map(GetExpensesForSettlementQueryResult source)
    {
        return new(
            source.Expenses.Select(expense => new GetExpensesForSettlementResponseExpense(
                expense.Id,
                expense.Title,
                expense.Amount,
                expense.PayingParticipantId,
                expense.PaymentDate
            ))
        );
    }

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Settlements()
            .MapGet("{settlementId}/expenses", (
                Guid settlementId,
                IHandler<
                    Guid,
                    GetExpensesForSettlementQuery,
                    GetExpensesForSettlementQueryResult,
                    GetExpensesForSettlementResponse
                > handler) => handler.Handle(settlementId)
            );
    }
}
