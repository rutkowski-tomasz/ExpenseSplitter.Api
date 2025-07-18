using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Settlements.CalculateReimbursement;

public sealed record CalculateReimbursementQuery(Guid SettlementId) : IQuery<CalculateReimbursementQueryResult>;
