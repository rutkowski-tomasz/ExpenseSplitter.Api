using ExpenseSplitter.Api.Application.Users.LoginUser;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;

namespace ExpenseSplitter.Api.Presentation.Users;

public sealed record UserLoginRequest(string Email, string Password);

public sealed record LoginUserResponse(string AccessToken);

public class UserLoginEndpoint : IEndpoint,
    IMapper<UserLoginRequest, LoginUserCommand>,
    IMapper<LoginUserResult, LoginUserResponse>
{
    public LoginUserCommand Map(UserLoginRequest source)
    {
        return new LoginUserCommand(
            source.Email,
            source.Password
        );
    }

    public LoginUserResponse Map(LoginUserResult source)
    {
        return new LoginUserResponse(
            source.AccessToken
        );
    }

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Users()
            .MapPost("login", (
                UserLoginRequest request,
                IHandler<
                    UserLoginRequest,
                    LoginUserCommand,
                    LoginUserResult,
                    LoginUserResponse
                > handler) => handler.Handle(request)
            )
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status401Unauthorized);
    }
}
