using ExpenseSplitter.Api.Application.Users.GetLoggedInUser;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;

namespace ExpenseSplitter.Api.Presentation.Users;

public sealed record UserMeResponse(
    Guid Id,
    string Email,
    string Nickname
);

public class UserMeEndpoint : IEndpoint,
    IMapper<GetLoggedInUserQueryResult, UserMeResponse>
{
    public UserMeResponse Map(GetLoggedInUserQueryResult source)
    {
        return new UserMeResponse(
            source.Id,
            source.Email,
            source.Nickname
        );
    }

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Users()
            .RequireAuthorization()
            .MapGet("me", (
                IHandlerEmptyRequest<
                    GetLoggedInUserQuery,
                    GetLoggedInUserQueryResult,
                    UserMeResponse
                > handler) => handler.Handle()
            )
            .Produces<string>(StatusCodes.Status400BadRequest);
    }
}
