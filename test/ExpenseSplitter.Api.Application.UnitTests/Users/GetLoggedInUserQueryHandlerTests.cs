using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Users.GetLoggedInUser;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Application.UnitTests.Users;

public class GetLoggedInUserQueryHandlerTests
{
    private readonly Mock<IUserRepository> userRepositoryMock;
    private readonly Mock<IUserContext> userContextMock;
    private readonly GetLoggedInUserQueryHandler handler;

    public GetLoggedInUserQueryHandlerTests()
    {
        userRepositoryMock = new Mock<IUserRepository>();
        userContextMock = new Mock<IUserContext>();

        handler = new GetLoggedInUserQueryHandler(userRepositoryMock.Object, userContextMock.Object);
    }
    
    [Fact]
    public async Task Handle_ShouldSuccess()
    {
        var userId = new Fixture().Create<UserId>();
        userContextMock.Setup(x => x.UserId).Returns(userId);

        var user = new Fixture()
            .Build<User>()
            .FromFactory((string nickname, string email) => User.Create(nickname, email, userId).Value)
            .Create();

        userRepositoryMock
            .Setup(x => x.GetById(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        var query = new Fixture().Create<GetLoggedInUserQuery>();

        var result = await handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(user.Id.Value);
        result.Value.Email.Should().Be(user.Email);
        result.Value.Nickname.Should().Be(user.Nickname);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenIdentityIdIsEmpty()
    {
        var query = new Fixture().Create<GetLoggedInUserQuery>();

        var result = await handler.Handle(query, default);

        result.IsFailure.Should().BeTrue();
    }
    
    [Fact]
    public async Task Handle_ShouldFail_WhenUserDoesntExistInUserRepository()
    {
        var userId = new Fixture().Create<UserId>();
        userContextMock.Setup(x => x.UserId).Returns(userId);
        
        var query = new Fixture().Create<GetLoggedInUserQuery>();

        var result = await handler.Handle(query, default);

        result.IsFailure.Should().BeTrue();
    }
}
