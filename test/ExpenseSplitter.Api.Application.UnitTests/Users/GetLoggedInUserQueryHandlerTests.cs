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
        userContextMock.Setup(x => x.IdentityId).Returns("my-id");

        var user = new Fixture()
            .Build<User>()
            .FromFactory((string nickname, string email) => User.Create(nickname, email).Value)
            .Do(x => x.SetIdentityId(Guid.NewGuid().ToString()))
            .Create();

        userRepositoryMock
            .Setup(x => x.GetByIdentityId("my-id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        var query = new Fixture().Create<GetLoggedInUserQuery>();

        var result = await handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(user.IdentityId);
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
        userContextMock.Setup(x => x.IdentityId).Returns("my-id");
        
        var query = new Fixture().Create<GetLoggedInUserQuery>();

        var result = await handler.Handle(query, default);

        result.IsFailure.Should().BeTrue();
    }
}
