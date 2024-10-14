namespace ExpenseSplitter.Api.Infrastructure.Authentication;

public sealed class AuthenticationOptions
{
    public string BaseUrl { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;

    public string MetadataUrlPath { get; init; } = string.Empty;

    public bool RequireHttpsMetadata { get; init; }

    public string ValidIssuerPath { get; init; } = string.Empty;
}
