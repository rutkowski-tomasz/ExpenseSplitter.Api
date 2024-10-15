using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.Repositories;

internal sealed class ParticipantRepository(ApplicationDbContext dbContext)
    : Repository<Participant, ParticipantId>(dbContext), IParticipantRepository
{
    public Task<bool> IsParticipantInSettlement(SettlementId settlementId, ParticipantId participantId, CancellationToken cancellationToken)
    {
        return DbContext
            .Set<Participant>()
            .AnyAsync(x => x.SettlementId == settlementId && x.Id == participantId, cancellationToken);
    }

    public async Task<bool> AreAllParticipantsInSettlement(SettlementId settlementId, IEnumerable<ParticipantId> participantIds, CancellationToken cancellationToken)
    {
        var participants = await DbContext
            .Set<Participant>()
            .Where(x => x.SettlementId == settlementId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        return participantIds.All(x => participants.Contains(x));
    }

    public Task<List<Participant>> GetAllBySettlementId(SettlementId settlementId, CancellationToken cancellationToken)
    {
        return DbContext
            .Set<Participant>()
            .Where(x => x.SettlementId == settlementId)
            .ToListAsync(cancellationToken);
    }
}
