namespace ExpenseSplitter.Api.Presentation.Models;

public sealed record RegisterUserRequest(
    string Email,
    string Nickname,
    string Password
);
