using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Expenses.CreateExpense;

public sealed record CreateExpenseCommand(
    string Name,
    Guid SettlementId,
    Guid PayingParticipantId
) : ICommand<Guid>;
