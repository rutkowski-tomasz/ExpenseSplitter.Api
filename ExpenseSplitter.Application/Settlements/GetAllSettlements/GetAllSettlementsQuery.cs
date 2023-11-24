using ExpenseSplitter.Application.Abstractions.Cqrs;
using ExpenseSplitter.Application.Settlements.GetSettlement;

namespace ExpenseSplitter.Application.Settlements.GetAllSettlements;

public sealed record GetAllSettlementsQuery() : IQuery<IEnumerable<GetSettlementResponse>>;