using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Participants.GetParticipantsBySettlementId;

internal sealed class GetParticipantsBySettlementIdQueryHandler : IQueryHandler<GetParticipantsBySettlementIdQuery, GetParticipantsBySettlementIdQueryResult>
{
    private readonly ISettlementUserRepository settlementUserRepository;
    private readonly IParticipantRepository participantRepository;

    public GetParticipantsBySettlementIdQueryHandler(
        ISettlementUserRepository settlementUserRepository,
        IParticipantRepository participantRepository
    )
    {
        this.settlementUserRepository = settlementUserRepository;
        this.participantRepository = participantRepository;
    }

    public async Task<Result<GetParticipantsBySettlementIdQueryResult>> Handle(GetParticipantsBySettlementIdQuery request, CancellationToken cancellationToken)
    {
        var settlementId = new SettlementId(request.settlementId);
        if (!await settlementUserRepository.CanUserAccessSettlement(settlementId, cancellationToken))
        {
            return Result.Failure<GetParticipantsBySettlementIdQueryResult>(SettlementErrors.Forbidden);
        }

        var participants = await participantRepository.GetAllBySettlementId(settlementId, cancellationToken);

        var result = new GetParticipantsBySettlementIdQueryResult(
            participants.Select(x => new GetParticipantsBySettlementIdQueryResultParticipant(
                x.Id,
                x.Nickname
            ))
        );

        return result;
    }
}