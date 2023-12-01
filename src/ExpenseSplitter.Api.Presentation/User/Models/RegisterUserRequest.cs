namespace ExpenseSplitter.Api.Presentation.User.Models;

public sealed record RegisterUserRequest(
    string Email,
    string Nickname,
    string Password
);
