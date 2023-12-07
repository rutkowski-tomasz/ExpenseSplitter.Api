using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Participants.ClaimParticipant;

public sealed record ClaimParticipantCommand(
    Guid SettlementId,
    Guid ParticipantId
) : ICommand;
