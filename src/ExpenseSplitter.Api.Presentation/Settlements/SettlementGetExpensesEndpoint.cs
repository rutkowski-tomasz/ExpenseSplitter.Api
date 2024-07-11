using ExpenseSplitter.Api.Application.Expenses.GetExpensesForSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;

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

public class SettlementGetExpensesEndpoint() : Endpoint<Guid, GetExpensesForSettlementQuery, GetExpensesForSettlementQueryResult, GetExpensesForSettlementResponse>(
    Route: "{settlementId}/expenses",
    Group: EndpointGroup.Settlements,
    Method: EndpointMethod.Get,
    MapRequest: request => new(request),
    MapResponse: result => new(
        result.Expenses.Select(expense => new GetExpensesForSettlementResponseExpense(
            expense.Id,
            expense.Title,
            expense.Amount,
            expense.PayingParticipantId,
            expense.PaymentDate
        ))
    ),
    ErrorStatusCodes: [
        StatusCodes.Status403Forbidden,
        StatusCodes.Status404NotFound
    ]
);
