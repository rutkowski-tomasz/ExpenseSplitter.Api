using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Application.Settlements.GetSettlement;

namespace ExpenseSplitter.Api.Application.Settlements.GetAllSettlements;

public sealed record GetAllSettlementsQuery : IQuery<IEnumerable<GetSettlementResponse>>;