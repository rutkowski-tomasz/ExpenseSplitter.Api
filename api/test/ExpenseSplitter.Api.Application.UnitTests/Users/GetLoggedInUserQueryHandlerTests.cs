using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Users.GetLoggedInUser;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Application.UnitTests.Users;

public class GetLoggedInUserQueryHandlerTests
{
    private readonly Fixture fixture = CustomFixture.Create();
    private readonly IUserRepository userRepository = Substitute.For<IUserRepository>();
    private readonly IUserContext userContext = Substitute.For<IUserContext>();
    private readonly GetLoggedInUserQueryHandler handler;

    public GetLoggedInUserQueryHandlerTests()
    {
        handler = new GetLoggedInUserQueryHandler(userRepository, userContext);
    }
    
    [Fact]
    public async Task Handle_ShouldSuccess()
    {
        var user = fixture.Create<User>();
        userContext.UserId.Returns(user.Id);

        userRepository
            .GetById(user.Id, Arg.Any<CancellationToken>())
            .Returns(user);
        
        var query = fixture.Create<GetLoggedInUserQuery>();

        var result = await handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(user.Id.Value);
        result.Value.Email.Should().Be(user.Email);
        result.Value.Nickname.Should().Be(user.Nickname);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenIdentityIdIsEmpty()
    {
        var query = fixture.Create<GetLoggedInUserQuery>();

        var result = await handler.Handle(query, default);

        result.IsFailure.Should().BeTrue();
    }
    
    [Fact]
    public async Task Handle_ShouldFail_WhenUserDoesntExistInUserRepository()
    {
        var userId = fixture.Create<UserId>();
        userContext.UserId.Returns(userId);
        
        var query = fixture.Create<GetLoggedInUserQuery>();

        var result = await handler.Handle(query, default);

        result.IsFailure.Should().BeTrue();
    }
}
