namespace ExpenseSplitter.Api.Infrastructure.Authentication.Models;

public sealed class UserRepresentationModel
{
    private UserRepresentationModel() { }

    public Dictionary<string, List<string>>? Attributes { get; set; }
    public long? CreatedTimestamp { get; set; }
    public CredentialRepresentationModel[]? Credentials { get; set; }
    public string? Email { get; set; }
    public bool? EmailVerified { get; set; }
    public bool? Enabled { get; set; }
    public string[]? RequiredActions { get; set; }
    public string? Username { get; set; }

    internal static UserRepresentationModel Create(string email) =>
        new()
        {
            Email = email,
            Username = email,
            Enabled = true,
            EmailVerified = true,
            CreatedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Attributes = [],
            RequiredActions = []
        };
}
