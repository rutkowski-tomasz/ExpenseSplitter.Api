using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Application.Settlements.GetSettlement;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Application.Settlements.GetAllSettlements;

internal sealed class GetSettlementsQueryHandler : IQueryHandler<GetAllSettlementsQuery, GetAllSettlementsQueryResponse>
{
    private readonly ISettlementRepository settlementRepository;

    public GetSettlementsQueryHandler(ISettlementRepository settlementRepository)
    {
        this.settlementRepository = settlementRepository;
    }

    public async Task<Result<GetAllSettlementsQueryResponse>> Handle(GetAllSettlementsQuery _, CancellationToken cancellationToken)
    {
        var settlements = await settlementRepository.GetAllAsync(cancellationToken);

        var response = new GetAllSettlementsQueryResponse(
            settlements.Select(settlement => new GetAllSettlementsQueryResponseSettlement(
                settlement.Id.Value,
                settlement.Name
            ))
        );

        return Result.Success(response);
    }
}