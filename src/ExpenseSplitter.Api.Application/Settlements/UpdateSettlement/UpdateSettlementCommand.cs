using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Settlements.UpdateSettlement;

public sealed record UpdateSettlementCommand(
    Guid Id,
    string Name
) : ICommand;
