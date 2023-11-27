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
}