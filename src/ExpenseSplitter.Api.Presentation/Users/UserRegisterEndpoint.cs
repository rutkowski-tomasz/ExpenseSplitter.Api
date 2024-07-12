using ExpenseSplitter.Api.Application.Users.RegisterUser;
using ExpenseSplitter.Api.Presentation.Abstractions;

namespace ExpenseSplitter.Api.Presentation.Users;

public sealed record UserRegisterRequest(
    string Email,
    string Nickname,
    string Password
);

public class UserRegisterEndpoint() : Endpoint<UserRegisterRequest, RegisterUserCommand>(
    Endpoints.Users.Post("register").ProducesErrorCodes(
        StatusCodes.Status400BadRequest,
        StatusCodes.Status401Unauthorized
    ),
    request => new RegisterUserCommand(
        request.Email,
        request.Nickname,
        request.Password
    )
);
