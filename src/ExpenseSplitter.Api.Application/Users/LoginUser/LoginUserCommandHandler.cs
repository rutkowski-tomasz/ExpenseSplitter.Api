using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Application.Users.LoginUser;

internal sealed class LoginUserCommandHandler(IJwtService service)
    : ICommandHandler<LoginUserCommand, LoginUserResult>
{
    public async Task<Result<LoginUserResult>> Handle(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await service.GetAccessTokenAsync(command.Email, command.Password, cancellationToken);

        return result.IsFailure
            ? Result.Failure<LoginUserResult>(result.AppError)
            : new LoginUserResult(result.Value);
    }
}
