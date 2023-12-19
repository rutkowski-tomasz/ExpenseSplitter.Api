using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Application.Settlements.GetAllSettlements;

internal sealed class GetSettlementsQueryHandler : IQueryHandler<GetAllSettlementsQuery, GetAllSettlementsQueryResult>
{
    private readonly ISettlementRepository settlementRepository;

    public GetSettlementsQueryHandler(ISettlementRepository settlementRepository)
    {
        this.settlementRepository = settlementRepository;
    }

    public async Task<Result<GetAllSettlementsQueryResult>> Handle(GetAllSettlementsQuery query, CancellationToken cancellationToken)
    {
        var settlements = await settlementRepository.GetAll(query.Page, query.PageSize, cancellationToken);

        var response = new GetAllSettlementsQueryResult(
            settlements.Select(settlement => new GetAllSettlementsQueryResultSettlement(
                settlement.Id.Value,
                settlement.Name
            ))
        );

        return Result.Success(response);
    }
}