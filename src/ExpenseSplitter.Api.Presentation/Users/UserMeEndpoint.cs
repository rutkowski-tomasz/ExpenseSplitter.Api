using ExpenseSplitter.Api.Application.Users.GetLoggedInUser;
using ExpenseSplitter.Api.Presentation.MediatrEndpoints;

namespace ExpenseSplitter.Api.Presentation.Users;

public sealed record UserMeResponse(
    Guid Id,
    string Email,
    string Nickname
);

public class UserMeEndpoint() : Endpoint<GetLoggedInUserQuery, GetLoggedInUserQueryResult, UserMeResponse>(
    Endpoints.Users.Get("me").ProducesErrorCodes(
        StatusCodes.Status400BadRequest
    ),
    result => new UserMeResponse(
        result.Id,
        result.Email,
        result.Nickname
    )
);
