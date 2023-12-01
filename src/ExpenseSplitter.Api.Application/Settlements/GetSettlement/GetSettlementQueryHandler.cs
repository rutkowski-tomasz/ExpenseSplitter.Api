using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Application.Settlements.GetSettlement;

internal sealed class GetSettlementQueryHandler : IQueryHandler<GetSettlementQuery, GetSettlementResponse>
{
    private readonly ISettlementRepository settlementRepository;

    public GetSettlementQueryHandler(ISettlementRepository settlementRepository)
    {
        this.settlementRepository = settlementRepository;
    }

    public async Task<Result<GetSettlementResponse>> Handle(GetSettlementQuery request, CancellationToken cancellationToken)
    {
        var settlement = await settlementRepository.GetByIdAsync(new SettlementId(request.SettlementId), cancellationToken);

        if (settlement is null)
        {
            return Result.Failure<GetSettlementResponse>(SettlementErrors.NotFound);
        }

        var settlementDto = new GetSettlementResponse(
            settlement.Id.Value,
            settlement.Name,
            settlement.InviteCode
        );

        return settlementDto;
    }
}