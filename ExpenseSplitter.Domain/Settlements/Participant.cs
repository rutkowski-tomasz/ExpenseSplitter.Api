using ExpenseSplitter.Domain.Expenses;
using ExpenseSplitter.Domain.Users;

namespace ExpenseSplitter.Domain.Settlements;

public class Participant
{
    private Participant()
    {

    }

    public ParticipantId ParticipantId { get; set; }

    public SettlementId SettlementId { get; private set; }

    public UserId UserId { get; private set; }

    public string Nickname { get; private set; } = string.Empty;

    private readonly HashSet<ExpenseAllocationParticipant> expenseAllocationParticipants = new();

    public static Participant Create(SettlementId settlementId, UserId userId, string nickname)
    {
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
