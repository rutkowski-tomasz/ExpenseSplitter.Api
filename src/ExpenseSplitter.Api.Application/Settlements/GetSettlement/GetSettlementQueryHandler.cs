using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Settlements.GetSettlement;

internal sealed class GetSettlementQueryHandler : IQueryHandler<GetSettlementQuery, GetSettlementQueryResult>
{
    private readonly ISettlementRepository settlementRepository;
    private readonly ISettlementUserRepository settlementUserRepository;
    private readonly IParticipantRepository participantRepository;

    public GetSettlementQueryHandler(
        ISettlementRepository settlementRepository,
        ISettlementUserRepository settlementUserRepository,
        IParticipantRepository participantRepository
    )
    {
        this.settlementRepository = settlementRepository;
        this.settlementUserRepository = settlementUserRepository;
        this.participantRepository = participantRepository;
    }

    public async Task<Result<GetSettlementQueryResult>> Handle(GetSettlementQuery request, CancellationToken cancellationToken)
    {
        var settlementId = new SettlementId(request.SettlementId);
        if (!await settlementUserRepository.CanUserAccessSettlement(settlementId, cancellationToken))
        {
            return Result.Failure<GetSettlementQueryResult>(SettlementErrors.Forbidden);
        }

        var settlement = await settlementRepository.GetById(settlementId, cancellationToken);
        if (settlement is null)
        {
            return Result.Failure<GetSettlementQueryResult>(SettlementErrors.NotFound);
        }

        var participants = await participantRepository.GetAllBySettlementId(settlementId, cancellationToken);

        var settlementDto = new GetSettlementQueryResult(
            settlement.Id.Value,
            settlement.Name,
            settlement.InviteCode,
            participants.Select(x => new GetSettlementQueryResultParticipant(
                x.Id.Value,
                x.Nickname
            ))
        );

        return settlementDto;
    }
}