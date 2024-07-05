using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Participants;

public static class ParticipantErrors
{
    public static readonly Error NicknameEmpty = new(
        ErrorType.Validation,
        "Can't create participant with empty nickname"
    );

    public static readonly Error NotFound = new(
        ErrorType.Validation,
        "Can't find participant with given id"
    );
}
