using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Users;

public static class UserErrors
{
    public static readonly Error EmptyNickname = new(
        "User.EmptyNickname",
        "Can't create user with empty nickname"
    );

    public static readonly Error InvalidCredentials = new(
        "User.InvalidCredentials",
        "The provided credentials were invalid"
    );

    public static readonly Error NotFound = new(
        "User.NotFound",
        "The user with the specified identifier was not found"
    );
}