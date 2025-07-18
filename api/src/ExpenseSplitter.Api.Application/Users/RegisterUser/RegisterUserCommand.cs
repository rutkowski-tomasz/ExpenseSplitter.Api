using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Users.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string Nickname,
    string Password
) : ICommand<Guid>;
