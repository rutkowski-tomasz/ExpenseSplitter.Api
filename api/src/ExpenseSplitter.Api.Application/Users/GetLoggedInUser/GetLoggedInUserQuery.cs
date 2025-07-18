using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Users.GetLoggedInUser;

public sealed record GetLoggedInUserQuery : IQuery<GetLoggedInUserQueryResult>;
