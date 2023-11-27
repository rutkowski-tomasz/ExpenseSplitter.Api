namespace ExpenseSplitter.Api.Application.Users.GetLoggedInUser;

public sealed class GetLoggedInUserResponse
{
    public Guid Id { get; init; }

    public string Email { get; init; } = string.Empty;

    public string Nickname { get; init; } = string.Empty;
}
