using ExpenseSplitter.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Application.Settlements.CreateSettlement;

public sealed record CreateSettlementCommand(
    string Name
) : ICommand<Guid>;
