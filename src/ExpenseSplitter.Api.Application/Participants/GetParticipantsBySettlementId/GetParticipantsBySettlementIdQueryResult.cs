using ExpenseSplitter.Api.Domain.Participants;

namespace ExpenseSplitter.Api.Application.Participants.GetParticipantsBySettlementId;

public sealed record GetParticipantsBySettlementIdQueryResult(
    IEnumerable<GetParticipantsBySettlementIdQueryResultParticipant> Participants
);

public sealed record GetParticipantsBySettlementIdQueryResultParticipant(
    ParticipantId Id,
    string Nickname
);
