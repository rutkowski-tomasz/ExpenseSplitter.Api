using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Application.Users.LoginUser;

internal sealed class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, LoginUserResult>
{
    private readonly IJwtService jwtService;

    public LoginUserCommandHandler(
        IJwtService jwtService
    )
    {
        this.jwtService = jwtService;
    }

    public async Task<Result<LoginUserResult>> Handle(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await jwtService.GetAccessTokenAsync(command.Email, command.Password, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<LoginUserResult>(result.Error);
        }

        return new LoginUserResult(result.Value);
    }
}
