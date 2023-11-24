using ExpenseSplitter.Domain.Settlements;
using ExpenseSplitter.Domain.Users;

namespace ExpenseSplitter.Domain.Participants;

public class Participant
{
    private Participant()
    {

    }

    public ParticipantId ParticipantId { get; set; }

    public SettlementId SettlementId { get; private set; }

    public UserId UserId { get; private set; }

    public string Nickname { get; private set; } = string.Empty;

    public static Participant Create(SettlementId settlementId, UserId userId, string nickname)
    {
        if (string.IsNullOrWhiteSpace(nickname))
        {
            throw new ArgumentException($"{nameof(nickname)} can't be empty or whitespace");
        }

        var participant = new Participant()
        {
            ParticipantId = new ParticipantId(Guid.NewGuid()),
            SettlementId = settlementId,
            UserId = userId,
            Nickname = nickname,
        };

        return participant;
    }
}
