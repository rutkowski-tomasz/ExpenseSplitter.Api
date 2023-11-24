using ExpenseSplitter.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Application.Settlements.GetSettlement;

public sealed record GetSettlementQuery(Guid SettlementId) : IQuery<GetSettlementResponse>;