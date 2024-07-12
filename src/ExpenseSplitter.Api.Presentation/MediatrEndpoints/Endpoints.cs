using ExpenseSplitter.Api.Presentation.Extensions;

namespace ExpenseSplitter.Api.Presentation.MediatrEndpoints;

public static class Endpoints
{
    public static AutoEndpointBuilder Expenses = new(builder => builder
        .MapGroup(nameof(Application.Expenses).ToLower())
        .WithTags(nameof(Application.Expenses))
        .RequireAuthorization()
    );

    public static AutoEndpointBuilder Settlements = new(builder => builder
        .MapGroup(nameof(Application.Settlements).ToLower())
        .WithTags(nameof(Application.Settlements))
        .RequireAuthorization()
    );

    public static AutoEndpointBuilder Users = new(builder => builder
        .MapGroup(nameof(Application.Users).ToLower())
        .WithTags(nameof(Application.Users))
        .RequireRateLimiting(RateLimitingExtensions.IpRateLimiting)
    );
}
