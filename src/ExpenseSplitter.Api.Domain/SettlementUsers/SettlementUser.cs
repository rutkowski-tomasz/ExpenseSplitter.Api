using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Domain.SettlementUsers;

public sealed class SettlementUser : Entity<SettlementUserId>
{
    private SettlementUser(
        SettlementUserId id,
        SettlementId settlementId,
        UserId userId
    ) : base(id)
    {
        SettlementId = settlementId;
        UserId = userId;
    }

    public SettlementId SettlementId { get; private set; }
    public UserId UserId { get; private set; }
    public ParticipantId? ParticipantId { get; private set; }

    public static Result<SettlementUser> Create(SettlementId settlementId, UserId userId)
    {
        var settlementUser = new SettlementUser(SettlementUserId.New(), settlementId, userId);
        return settlementUser;
    }

    public void SetParticipantId(ParticipantId participantId)
    {
        ParticipantId = participantId;
    }
}
