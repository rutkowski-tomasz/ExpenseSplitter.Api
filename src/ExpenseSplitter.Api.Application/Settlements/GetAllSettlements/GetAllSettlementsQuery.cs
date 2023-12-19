using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Settlements.GetAllSettlements;

public sealed record GetAllSettlementsQuery(int Page, int PageSize) : IQuery<GetAllSettlementsQueryResult>;