using ExpenseSplitter.Application.Abstractions.Cqrs;
using ExpenseSplitter.Domain.Abstractions;
using ExpenseSplitter.Domain.Settlements;

namespace ExpenseSplitter.Application.Settlements.GetSettlement;

public class GetSettlementQueryHandler : IQueryHandler<GetSettlementQuery, GetSettlementResponse>
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

        var settlementDto = new GetSettlementResponse
        {
            Id = settlement.Id.Value,
            Name = settlement.Name
        };

        return settlementDto;
    }
}