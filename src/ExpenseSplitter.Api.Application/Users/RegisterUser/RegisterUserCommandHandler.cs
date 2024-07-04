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
        return (await authenticationService.RegisterAsync(request.Email, request.Password, cancellationToken))
            .Bind(identityId => User.Create(request.Nickname, request.Email, new UserId(Guid.Parse(identityId))))
            .Tap(userRepository.Add)
            .Tap(async _ => await unitOfWork.SaveChangesAsync(cancellationToken))
            .Bind(user => Result.Success(user.Id.Value));
    }
}
