using ExpenseSplitter.Api.Application.Expenses.GetExpensesForSettlement;
using ExpenseSplitter.Api.Presentation.MediatrEndpoints;

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
    Endpoints.Settlements.Get("{settlementId}/expenses").ProducesErrorCodes(
        StatusCodes.Status403Forbidden,
        StatusCodes.Status404NotFound
    ),
    request => new(request),
    result => new(
        result.Expenses.Select(expense => new GetExpensesForSettlementResponseExpense(
            expense.Id,
            expense.Title,
            expense.Amount,
            expense.PayingParticipantId,
            expense.PaymentDate
        ))
    )
);
