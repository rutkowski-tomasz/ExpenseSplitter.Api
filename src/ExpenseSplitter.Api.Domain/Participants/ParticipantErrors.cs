using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Participants;

public class ParticipantErrors
{
    public static Error NicknameEmpty = new(
        "Participant.NicknameEmpty",
        "Can't create participant with empty nickname"
    );
}