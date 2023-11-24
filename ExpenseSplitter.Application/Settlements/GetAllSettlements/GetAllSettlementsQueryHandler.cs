using ExpenseSplitter.Application.Abstractions.Cqrs;
using ExpenseSplitter.Application.Settlements.GetSettlement;
using ExpenseSplitter.Domain.Abstractions;
using ExpenseSplitter.Domain.Settlements;

namespace ExpenseSplitter.Application.Settlements.GetAllSettlements;

internal sealed class GetSettlementsQueryHandler : IQueryHandler<GetAllSettlementsQuery, IEnumerable<GetSettlementResponse>>
{
    private readonly ISettlementRepository settlementRepository;

    public GetSettlementsQueryHandler(ISettlementRepository settlementRepository)
    {
        this.settlementRepository = settlementRepository;
    }

    public async Task<Result<IEnumerable<GetSettlementResponse>>> Handle(GetAllSettlementsQuery _, CancellationToken cancellationToken)
    {
        var settlements = await settlementRepository.GetAllAsync(cancellationToken);

        var settlementDto = settlements.Select(settlement => new GetSettlementResponse
        {
            Id = settlement.Id.Value,
            Name = settlement.Name
        });

        return Result.Success(settlementDto);
    }
}