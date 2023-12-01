namespace ExpenseSplitter.Api.Application.Settlements.GetAllSettlements;

public sealed record GetAllSettlementsQueryResponse(
    IEnumerable<GetAllSettlementsQueryResponseSettlement> Settlements
);

public sealed record GetAllSettlementsQueryResponseSettlement(
    Guid Id,
    string Name
);
