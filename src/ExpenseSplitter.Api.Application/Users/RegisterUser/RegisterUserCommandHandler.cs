using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Application.Users.RegisterUser;

internal sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly IAuthenticationService authenticationService;
    private readonly IUserRepository userRepository;
    private readonly IUnitOfWork unitOfWork;

    public RegisterUserCommandHandler(
        IAuthenticationService authenticationService,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        this.authenticationService = authenticationService;
        this.userRepository = userRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var identityIdResult = await authenticationService.RegisterAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (identityIdResult.IsFailure)
        {
            return Result.Failure<Guid>(identityIdResult.Error);
        }
        
        var identityId = identityIdResult.Value;
            
        var userResult = User.Create(request.Nickname, request.Email, new UserId(Guid.Parse(identityId)));

        if (userResult.IsFailure)
        {
            return Result.Failure<Guid>(userResult.Error);
        }

        var user = userResult.Value;

        userRepository.Add(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id.Value;
    }
}
