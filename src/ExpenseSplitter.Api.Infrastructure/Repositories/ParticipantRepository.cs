using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.Repositories;

internal sealed class ParticipantRepository : Repository<Participant, ParticipantId>, IParticipantRepository
{
    private readonly IUserContext userContext;

    public ParticipantRepository(
        ApplicationDbContext dbContext,
        IUserContext userContext
    ) : base(dbContext)
    {
        this.userContext = userContext;
    }

    public async Task<bool> IsUserParticipatingInSettlement(SettlementId settlementId, CancellationToken cancellationToken)
    {
        return await DbContext
            .Set<Participant>()
            .AnyAsync(x => x.SettlementId == settlementId && x.UserId == userContext.UserId, cancellationToken);
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
}
