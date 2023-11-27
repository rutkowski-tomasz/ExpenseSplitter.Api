using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Users;

public class UserErrors
{
    public static Error EmptyNickname = new(
        "User.EmptyNickname",
        "Can't create user with empty nickname"
    );

    public static Error InvalidCredentials = new(
        "User.InvalidCredentials",
        "The provided credentials were invalid"
    );

    public static Error NotFound = new(
        "User.NotFound",
        "The user with the specified identifier was not found"
    );
}