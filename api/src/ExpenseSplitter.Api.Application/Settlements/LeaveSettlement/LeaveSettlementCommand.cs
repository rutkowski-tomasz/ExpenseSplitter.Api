using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Settlements.LeaveSettlement;

public sealed record LeaveSettlementCommand(
    Guid SettlemetId
) : ICommand;
