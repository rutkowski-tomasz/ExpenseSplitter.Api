namespace ExpenseSplitter.Api.Application.Settlements.GetSettlement;

public sealed record GetSettlementResponse(
    Guid Id,
    string Name,
    string InviteCode,
    IEnumerable<GetSettlementResponseParticipant> Participants
);

public sealed record GetSettlementResponseParticipant(
    Guid Id,
    string Nickname
);