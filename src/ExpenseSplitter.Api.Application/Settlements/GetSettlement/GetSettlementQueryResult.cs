namespace ExpenseSplitter.Api.Application.Settlements.GetSettlement;

public sealed record GetSettlementQueryResult(
    Guid Id,
    string Name,
    string InviteCode,
    decimal TotalCost,
    decimal? YourCost,
    IEnumerable<GetSettlementQueryResultParticipant> Participants
);

public sealed record GetSettlementQueryResultParticipant(
    Guid Id,
    string Nickname
);