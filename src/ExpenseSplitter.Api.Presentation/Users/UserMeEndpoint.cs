using ExpenseSplitter.Api.Application.Users.GetLoggedInUser;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
using MediatR;

namespace ExpenseSplitter.Api.Presentation.Users;

public sealed record UserMeResponse(
    Guid Id,
    string Email,
    string Nickname
);

public class UserMeEndpoint
    : EndpointEmptyRequest<GetLoggedInUserQuery, GetLoggedInUserQueryResult, UserMeResponse>
{
    public override UserMeResponse MapResponse(GetLoggedInUserQueryResult source)
    {
        return new UserMeResponse(
            source.Id,
            source.Email,
            source.Nickname
        );
    }

    public override void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Users()
            .RequireAuthorization()
            .MapGet("me", (ISender sender) => Handle(sender))
            .Produces<string>(StatusCodes.Status400BadRequest);
    }
}
