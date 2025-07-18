using ExpenseSplitter.Api.Application.Users.RegisterUser;
using ExpenseSplitter.Api.Presentation.MediatrEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Presentation.Users;

public record UserRegisterRequest([FromBody] UserRegisterRequestBody Body);

public record UserRegisterRequestBody(
    string Email,
    string Nickname,
    string Password
);

public class UserRegisterEndpoint() : Endpoint<UserRegisterRequest, RegisterUserCommand, Guid, Guid>(
    Endpoints.Users.Post("register").ProducesErrorCodes([
        StatusCodes.Status400BadRequest,
        StatusCodes.Status401Unauthorized
    ]),
    request => new RegisterUserCommand(
        request.Body.Email,
        request.Body.Nickname,
        request.Body.Password
    ),
    result => result
);
