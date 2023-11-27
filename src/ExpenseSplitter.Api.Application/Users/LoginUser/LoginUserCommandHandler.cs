﻿using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Application.Users.LoginUser;

internal sealed class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, LoginUserResponse>
{
    private readonly IJwtService jwtService;

    public LoginUserCommandHandler(
        IJwtService jwtService
    )
    {
        this.jwtService = jwtService;
    }

    public async Task<Result<LoginUserResponse>> Handle(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await jwtService.GetAccessTokenAsync(command.Email, command.Password, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<LoginUserResponse>(UserErrors.InvalidCredentials);
        }

        return new LoginUserResponse(result.Value);
    }
}