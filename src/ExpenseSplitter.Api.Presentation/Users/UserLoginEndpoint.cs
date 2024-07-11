using ExpenseSplitter.Api.Application.Users.LoginUser;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
using MediatR;

namespace ExpenseSplitter.Api.Presentation.Users;

public sealed record UserLoginRequest(string Email, string Password);

public sealed record LoginUserResponse(string AccessToken);

public class UserLoginEndpoint
    : Endpoint<UserLoginRequest, LoginUserCommand, LoginUserResult, LoginUserResponse>
{
    public override LoginUserCommand MapRequest(UserLoginRequest source)
    {
        return new LoginUserCommand(
            source.Email,
            source.Password
        );
    }

    public override LoginUserResponse MapResponse(LoginUserResult source)
    {
        return new LoginUserResponse(
            source.AccessToken
        );
    }

    public override void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Users()
            .MapPost("login", (UserLoginRequest request, ISender sender)
                => Handle(request, sender))
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status401Unauthorized);
    }
}
