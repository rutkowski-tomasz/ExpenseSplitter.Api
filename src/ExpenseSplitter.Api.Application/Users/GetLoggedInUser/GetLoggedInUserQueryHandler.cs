using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Application.Users.GetLoggedInUser;

internal sealed class GetLoggedInUserQueryHandler
    : IQueryHandler<GetLoggedInUserQuery, GetLoggedInUserQueryResult>
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

    public async Task<Result<GetLoggedInUserQueryResult>> Handle(
        GetLoggedInUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetById(userContext.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<GetLoggedInUserQueryResult>(UserErrors.NotFound);
        }

        var response = new GetLoggedInUserQueryResult(
            user.Id.Value,
            user.Email,
            user.Nickname
        );
        
        return Result.Success(response);
    }
}