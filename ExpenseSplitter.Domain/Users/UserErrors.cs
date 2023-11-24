using ExpenseSplitter.Domain.Abstractions;

namespace ExpenseSplitter.Domain.Users;

public class UserErrors
{
    public static Error EmptyNickname = new(
        "User.EmptyNickname",
        "Can't create user with empty nickname"
    );
}