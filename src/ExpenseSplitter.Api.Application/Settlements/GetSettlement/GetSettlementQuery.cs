using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Settlements.GetSettlement;

public sealed record GetSettlementQuery(Guid SettlementId) : IQuery<GetSettlementQueryResult>;