namespace ExpenseSplitter.Api.Presentation.Extensions;

public static class RouteBuilderExtensions
{
    public static RouteGroupBuilder Settlements(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGroup(nameof(Application.Settlements).ToLower())
            .WithTags(nameof(Application.Settlements))
            .RequireAuthorization();
    }

    public static RouteGroupBuilder Users(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGroup(nameof(Application.Users).ToLower())
            .WithTags(nameof(Application.Users))
            .RequireRateLimiting(RateLimitingExtensions.IpRateLimiting);
    }

    public static RouteGroupBuilder Expenses(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGroup(nameof(Application.Expenses).ToLower())
            .WithTags(nameof(Application.Expenses))
            .RequireAuthorization();
    }
}
