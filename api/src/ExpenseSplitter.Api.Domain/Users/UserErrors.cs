using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Users;

public static class UserErrors
{
    public static readonly AppError EmptyNickname = new(
        ErrorType.Validation,
        "Can't create user with empty nickname"
    );

    public static readonly AppError InvalidCredentials = new(
        ErrorType.BadRequest,
        "The provided credentials were invalid"
    );

    public static readonly AppError NotFound = new(
        ErrorType.NotFound,
        "The user with the specified identifier was not found"
    );

    public static readonly AppError UserExists = new(
        ErrorType.Conflict,
        "The user with the specified email already exists"
    );
}
