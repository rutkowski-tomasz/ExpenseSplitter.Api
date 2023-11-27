using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Users.LoginUser;

public record LoginUserCommand(string Email, string Password) : ICommand<LoginUserResponse>;