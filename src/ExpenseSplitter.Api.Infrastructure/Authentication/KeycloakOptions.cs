namespace ExpenseSplitter.Api.Infrastructure.Authentication;

public sealed class KeycloakOptions
{
    public string BaseUrl { get; init; } = string.Empty;

    public string AdminUsersPath { get; init; } = string.Empty;

    public string TokenPath { get; init; } = string.Empty;

    public string AdminClientId { get; init; } = string.Empty;

    public string AdminClientSecret { get; init; } = string.Empty;

    public string AuthClientId { get; init; } = string.Empty;

    public string AuthClientSecret { get; init; } = string.Empty;
}
