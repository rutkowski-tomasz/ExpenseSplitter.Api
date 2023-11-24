using ExpenseSplitter.Domain.Abstractions;
using ExpenseSplitter.Domain.Settlements;
using ExpenseSplitter.Domain.Users;

namespace ExpenseSplitter.Domain.Participants;

public sealed class Participant : Entity<ParticipantId>
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

    public static Result<Participant> Create(SettlementId settlementId, UserId userId, string nickname)
    {
        if (string.IsNullOrWhiteSpace(nickname))
        {
            return Result.Failure<Participant>(ParticipantErrors.NicknameEmpty);
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

public class ParticipantErrors
{
    public static Error NicknameEmpty = new(
        "Participant.NicknameEmpty",
        "Can't create participant with empty nickname"
    );
}
