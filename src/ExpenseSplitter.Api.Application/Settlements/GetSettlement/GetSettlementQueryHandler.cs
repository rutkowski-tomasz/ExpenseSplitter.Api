using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Settlements.GetSettlement;

internal sealed class GetSettlementQueryHandler : IQueryHandler<GetSettlementQuery, GetSettlementResponse>
{
    private readonly ISettlementRepository settlementRepository;
    private readonly ISettlementUserRepository settlementUserRepository;

    public GetSettlementQueryHandler(
        ISettlementRepository settlementRepository,
        ISettlementUserRepository settlementUserRepository
    )
    {
        this.settlementRepository = settlementRepository;
        this.settlementUserRepository = settlementUserRepository;
    }

    public async Task<Result<GetSettlementResponse>> Handle(GetSettlementQuery request, CancellationToken cancellationToken)
    {
        var settlementId = new SettlementId(request.SettlementId);
        if (!await settlementUserRepository.CanUserAccessSettlement(settlementId, cancellationToken))
        {
            return Result.Failure<GetSettlementResponse>(SettlementErrors.Forbidden);
        }

        var settlement = await settlementRepository.GetByIdAsync(settlementId, cancellationToken);
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