using ExpenseSplitter.Api.Application.Users.LoginUser;
using ExpenseSplitter.Api.Presentation.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Presentation.Users;

public record UserLoginRequest([FromBody] UserLoginRequestBody Body);

public record UserLoginRequestBody(string Email, string Password);

public record LoginUserResponse(string AccessToken);

public class UserLoginEndpoint() : Endpoint<UserLoginRequest, LoginUserCommand, LoginUserResult, LoginUserResponse>(
    Endpoints.Users.Post("login").ProducesErrorCodes(
        StatusCodes.Status400BadRequest,
        StatusCodes.Status401Unauthorized
    ),
    request => new LoginUserCommand(request.Body.Email, request.Body.Password),
    result => new LoginUserResponse(result.AccessToken)
);
