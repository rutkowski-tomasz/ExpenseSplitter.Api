using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Participants.GetParticipantsBySettlementId;

public sealed record GetParticipantsBySettlementIdQuery(Guid settlementId) : IQuery<GetParticipantsBySettlementIdQueryResult>;
