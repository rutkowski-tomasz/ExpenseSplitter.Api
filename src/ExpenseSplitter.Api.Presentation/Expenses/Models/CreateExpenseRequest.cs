namespace ExpenseSplitter.Api.Presentation.Expenses.Models;

public record CreateExpenseRequest(
    string Name,
    Guid SettlementId,
    Guid PayingParticipantId
);