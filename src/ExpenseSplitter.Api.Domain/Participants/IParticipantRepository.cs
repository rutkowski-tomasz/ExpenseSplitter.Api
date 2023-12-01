using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Domain.Participants;

public interface IParticipantRepository
{
    Task<bool> IsUserParticipatingInSettlement(SettlementId settlementId, CancellationToken cancellationToken);

    Task<bool> AreAllParticipantsInSettlement(SettlementId settlementId, IEnumerable<ParticipantId> participantIds, CancellationToken cancellationToken);
}
