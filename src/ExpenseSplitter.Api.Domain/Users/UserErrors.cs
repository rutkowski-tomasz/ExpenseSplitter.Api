using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Users;

public static class UserErrors
{
    public static readonly Error EmptyNickname = new(
        ErrorType.Validation,
        "Can't create user with empty nickname"
    );

    public static readonly Error InvalidCredentials = new(
        ErrorType.BadRequest,
        "The provided credentials were invalid"
    );

    public static readonly Error NotFound = new(
        ErrorType.NotFound,
        "The user with the specified identifier was not found"
    );
}
