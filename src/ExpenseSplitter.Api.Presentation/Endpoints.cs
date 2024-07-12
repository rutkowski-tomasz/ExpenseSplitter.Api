using ExpenseSplitter.Api.Presentation.MediatrEndpoints;

namespace ExpenseSplitter.Api.Presentation;

public class Endpoints
{
    public static EndpointDefinition Root = EndpointDefinition.CreateGroup(builder => builder
        .MapGroup(string.Empty)
        .WithTags(typeof(Program).Assembly.GetName().Name!)
    );

    public static EndpointDefinition Expenses = EndpointDefinition.CreateGroup(builder => builder
        .MapGroup(nameof(Application.Expenses).ToLower())
        .WithTags(nameof(Application.Expenses))
        .RequireAuthorization()
    );

    public static EndpointDefinition Settlements = EndpointDefinition.CreateGroup(builder => builder
        .MapGroup(nameof(Application.Settlements).ToLower())
        .WithTags(nameof(Application.Settlements))
        .RequireAuthorization()
    );

    public static EndpointDefinition Users = EndpointDefinition.CreateGroup(builder => builder
        .MapGroup(nameof(Application.Users).ToLower())
        .WithTags(nameof(Application.Users))
    );
}
