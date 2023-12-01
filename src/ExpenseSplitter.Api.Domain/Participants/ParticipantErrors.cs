using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Participants;

public class ParticipantErrors
{
    public static Error NicknameEmpty = new(
        "Participant.NicknameEmpty",
        "Can't create participant with empty nickname"
    );

    public static Error NotFound = new(
        "Participant.NotFound",
        "Can't find participant with given id"
    );
}