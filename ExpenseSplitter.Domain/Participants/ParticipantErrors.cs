using ExpenseSplitter.Domain.Abstractions;

namespace ExpenseSplitter.Domain.Participants;

public class ParticipantErrors
{
    public static Error NicknameEmpty = new(
        "Participant.NicknameEmpty",
        "Can't create participant with empty nickname"
    );
}