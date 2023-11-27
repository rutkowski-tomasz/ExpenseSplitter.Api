using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Settlements.CreateSettlement;

public sealed record CreateSettlementCommand(
    string Name
) : ICommand<Guid>;
