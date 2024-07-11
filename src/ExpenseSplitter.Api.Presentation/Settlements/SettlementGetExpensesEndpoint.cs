using ExpenseSplitter.Api.Application.Expenses.GetExpensesForSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
using MediatR;

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


public class SettlementGetExpensesEndpoint : Endpoint<Guid, GetExpensesForSettlementQuery, GetExpensesForSettlementQueryResult, GetExpensesForSettlementResponse>
{
    public override GetExpensesForSettlementQuery MapRequest(Guid source)
    {
        return new(source);
    }

    public override GetExpensesForSettlementResponse MapResponse(GetExpensesForSettlementQueryResult source)
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

    public override void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Settlements()
            .MapGet("{settlementId}/expenses", (Guid settlementId, ISender sender)
                => Handle(settlementId, sender));
    }
}
