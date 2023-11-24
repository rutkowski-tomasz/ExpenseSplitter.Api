using ExpenseSplitter.Domain.Abstractions;
using ExpenseSplitter.Domain.Settlements;
using ExpenseSplitter.Domain.Users;

namespace ExpenseSplitter.Domain.Participants;

public class Participant : Entity<ParticipantId>
{
    private Participant(
        ParticipantId id,
        SettlementId settlementId,
        UserId userId,
        string nickname
    ) : base(id)
    {
        SettlementId = settlementId;
        UserId = userId;
        Nickname = nickname;
    }

    public SettlementId SettlementId { get; private set; }

    public UserId UserId { get; private set; }

    public string Nickname { get; private set; }

    public static Participant Create(SettlementId settlementId, UserId userId, string nickname)
    {
        if (string.IsNullOrWhiteSpace(nickname))
        {
            throw new ArgumentException($"{nameof(nickname)} can't be empty or whitespace");
        }

        var participant = new Participant(
            ParticipantId.New(),
            settlementId,
            userId,
            nickname
        );

        return participant;
    }
}
