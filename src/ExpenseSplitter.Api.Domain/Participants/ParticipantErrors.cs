using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Participants;

public static class ParticipantErrors
{
    public static readonly AppError NicknameEmpty = new(
        ErrorType.Validation,
        "Can't create participant with empty nickname"
    );

    public static readonly AppError NotFound = new(
        ErrorType.Validation,
        "Can't find participant with given id"
    );
}
