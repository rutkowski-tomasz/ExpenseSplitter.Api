using ExpenseSplitter.Api.Application.Users.LoginUser;
using ExpenseSplitter.Api.Presentation.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Presentation.Users;

public record UserLoginRequest([FromBody] UserLoginRequestBody Body);

public record UserLoginRequestBody(string Email, string Password);

public record LoginUserResponse(string AccessToken);

public class UserLoginEndpoint() : Endpoint<UserLoginRequest, LoginUserCommand, LoginUserResult, LoginUserResponse>(
    Route: "login",
    Group: EndpointGroup.Users,
    Method: EndpointMethod.Post,
    MapRequest: request => new LoginUserCommand(request.Body.Email, request.Body.Password),
    MapResponse: result => new LoginUserResponse(result.AccessToken),
    ErrorStatusCodes: [
        StatusCodes.Status400BadRequest,
        StatusCodes.Status401Unauthorized
    ]
);
