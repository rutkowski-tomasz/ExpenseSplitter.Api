namespace ExpenseSplitter.Api.Application.Users.GetLoggedInUser;

public sealed record GetLoggedInUserQueryResult(
    Guid Id,
    string Email,
    string Nickname
);
