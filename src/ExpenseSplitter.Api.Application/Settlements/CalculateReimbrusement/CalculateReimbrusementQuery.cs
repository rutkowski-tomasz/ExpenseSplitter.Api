using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Settlements.CalculateReimbrusement;

public sealed record CalculateReimbrusementQuery(Guid SettlementId) : IQuery<CalculateReimbrusementQueryResult>;