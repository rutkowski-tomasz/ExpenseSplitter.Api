using ExpenseSplitter.Api.Application.Users.RegisterUser;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;

namespace ExpenseSplitter.Api.Presentation.Users;

public sealed record UserRegisterRequest(
    string Email,
    string Nickname,
    string Password
);

public class UserRegisterEndpoint : IEndpoint,
    IMapper<UserRegisterRequest, RegisterUserCommand>
{
    public RegisterUserCommand Map(UserRegisterRequest source)
    {
        return new RegisterUserCommand(
            source.Email,
            source.Nickname,
            source.Password
        );
    }

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Users()
            .MapPost("register", (
                UserRegisterRequest request,
                IHandler<
                    UserRegisterRequest,
                    RegisterUserCommand,
                    Guid,
                    Guid
                > handler) => handler.Handle(request)
            )
            .Produces<string>(StatusCodes.Status400BadRequest);
    }
}
