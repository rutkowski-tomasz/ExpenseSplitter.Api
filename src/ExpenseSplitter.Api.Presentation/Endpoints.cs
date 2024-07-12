namespace ExpenseSplitter.Api.Presentation.MediatrEndpoints;

public partial record Endpoints
{
    public static Endpoints Expenses = CreateGroup(builder => builder
        .MapGroup(nameof(Application.Expenses).ToLower())
        .WithTags(nameof(Application.Expenses))
        .RequireAuthorization()
    );

    public static Endpoints Settlements = CreateGroup(builder => builder
        .MapGroup(nameof(Application.Settlements).ToLower())
        .WithTags(nameof(Application.Settlements))
        .RequireAuthorization()
    );

    public static Endpoints Users = CreateGroup(builder => builder
        .MapGroup(nameof(Application.Users).ToLower())
        .WithTags(nameof(Application.Users))
    );
}
