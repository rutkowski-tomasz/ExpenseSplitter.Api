using ExpenseSplitter.Api.Application.Users.GetLoggedInUser;
using ExpenseSplitter.Api.Presentation.Abstractions;

namespace ExpenseSplitter.Api.Presentation.Users;

public sealed record UserMeResponse(
    Guid Id,
    string Email,
    string Nickname
);

public class UserMeEndpoint() : Endpoint<GetLoggedInUserQuery, GetLoggedInUserQueryResult, UserMeResponse>(
    Route: "me",
    Group: EndpointGroup.Users,
    Method: EndpointMethod.Get,
    MapResponse: result => new UserMeResponse(
        result.Id,
        result.Email,
        result.Nickname
    ),
    ErrorStatusCodes: [
        StatusCodes.Status400BadRequest
    ]
);
