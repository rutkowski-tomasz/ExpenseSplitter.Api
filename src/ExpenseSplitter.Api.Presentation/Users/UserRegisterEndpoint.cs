using ExpenseSplitter.Api.Application.Users.RegisterUser;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
using MediatR;

namespace ExpenseSplitter.Api.Presentation.Users;

public sealed record UserRegisterRequest(
    string Email,
    string Nickname,
    string Password
);

public class UserRegisterEndpoint : EndpointEmptyResponse<UserRegisterRequest, RegisterUserCommand>
{
    public override RegisterUserCommand MapRequest(UserRegisterRequest source)
    {
        return new RegisterUserCommand(
            source.Email,
            source.Nickname,
            source.Password
        );
    }

    public override void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Users()
            .MapPost("register", (UserRegisterRequest request, ISender sender)
                => Handle(request, sender))
            .Produces<string>(StatusCodes.Status400BadRequest);
    }
}
