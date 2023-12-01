using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Application.Users.GetLoggedInUser;

internal sealed class GetLoggedInUserQueryHandler
    : IQueryHandler<GetLoggedInUserQuery, GetLoggedInUserResponse>
{
    private readonly IUserRepository userRepository;
    private readonly IUserContext userContext;

    public GetLoggedInUserQueryHandler(
        IUserRepository userRepository,
        IUserContext userContext)
    {
        this.userRepository = userRepository;
        this.userContext = userContext;
    }

    public async Task<Result<GetLoggedInUserResponse>> Handle(
        GetLoggedInUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userContext.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<GetLoggedInUserResponse>(UserErrors.NotFound);
        }

        var response = new GetLoggedInUserResponse
        {
            Email = user.Email,
            Nickname = user.Nickname,
            Id = user.Id.Value
        };
        
        return Result.Success(response);
    }
}