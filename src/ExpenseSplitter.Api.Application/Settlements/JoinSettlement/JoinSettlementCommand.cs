using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Settlements.JoinSettlement;

public sealed record JoinSettlementCommand(
    string InviteCode,
    string Nickname
) : ICommand<Guid>;
