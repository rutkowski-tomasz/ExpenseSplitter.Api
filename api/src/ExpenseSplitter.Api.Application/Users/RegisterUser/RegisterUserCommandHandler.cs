using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Application.Abstractions.Metrics;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ExpenseSplitter.Api.Application.Users.RegisterUser;

internal sealed class RegisterUserCommandHandler(
    IAuthenticationService authenticationService,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IMetricsService metricsService
) : ICommandHandler<RegisterUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken
    )
    {
        if (await userRepository.Exists(request.Email, cancellationToken))
        {
            return UserErrors.UserExists;
        }

        var identityIdResult = await authenticationService.RegisterAsync(request.Email, request.Password, cancellationToken);

        if (identityIdResult.IsFailure)
        {
            return identityIdResult.AppError;
        }

        var identityId = identityIdResult.Value;
        var userId = new UserId(Guid.Parse(identityId));
        var userResult = User.Create(request.Nickname, request.Email, userId);

        if (userResult.IsFailure)
        {
            return userResult.AppError;
        }

        var user = userResult.Value;
        userRepository.Add(user);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is NpgsqlException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            return UserErrors.UserExists;
        }

        metricsService.RecordRegisteredUser();

        var result = Result.Success(user.Id.Value);
        return result;
    }
}
