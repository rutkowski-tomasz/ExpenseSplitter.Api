namespace ExpenseSplitter.Api.Application.Settlements.GetSettlement;

public sealed record GetSettlementResponse(
    Guid Id,
    string Name,
    string InviteCode
);
