using ExpenseSplitter.Api.Presentation.MediatrEndpoints;

namespace ExpenseSplitter.Api.Presentation;

public static class Endpoints
{
    public static readonly EndpointDefinition Expenses = EndpointDefinition.CreateGroup(builder => builder
        .MapGroup(nameof(Application.Expenses))
        .WithTags(nameof(Application.Expenses))
        .RequireAuthorization()
    );

    public static readonly EndpointDefinition Settlements = EndpointDefinition.CreateGroup(builder => builder
        .MapGroup(nameof(Application.Settlements))
        .WithTags(nameof(Application.Settlements))
        .RequireAuthorization()
    );

    public static readonly EndpointDefinition Users = EndpointDefinition.CreateGroup(builder => builder
        .MapGroup(nameof(Application.Users))
        .WithTags(nameof(Application.Users))
    );
}
