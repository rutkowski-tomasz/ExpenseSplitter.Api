using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Users.LoginUser;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<LoginUserResult>;