using System.Collections.Concurrent;
using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Domain.Participants;

public interface IParticipantRepository
{
    void Add(Participant participant);

    Task<bool> IsParticipantInSettlement(SettlementId settlementId, ParticipantId participantId, CancellationToken cancellationToken);

    Task<bool> AreAllParticipantsInSettlement(SettlementId settlementId, IEnumerable<ParticipantId> participantIds, CancellationToken cancellationToken);

    Task<IEnumerable<Participant>> GetAllBySettlementId(SettlementId settlementId, CancellationToken cancellationToken);
}
