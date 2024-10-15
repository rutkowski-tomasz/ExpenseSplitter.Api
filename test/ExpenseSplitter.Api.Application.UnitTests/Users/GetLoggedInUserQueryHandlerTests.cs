using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Users.GetLoggedInUser;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Application.UnitTests.Users;

public class GetLoggedInUserQueryHandlerTests
{
    private readonly Fixture fixture;
    private readonly Mock<IUserRepository> userRepositoryMock;
    private readonly Mock<IUserContext> userContextMock;
    private readonly GetLoggedInUserQueryHandler handler;

    public GetLoggedInUserQueryHandlerTests()
    {
        fixture = CustomFixture.Create();
        userRepositoryMock = new Mock<IUserRepository>();
        userContextMock = new Mock<IUserContext>();

        handler = new GetLoggedInUserQueryHandler(userRepositoryMock.Object, userContextMock.Object);
    }
    
    [Fact]
    public async Task Handle_ShouldSuccess()
    {
        var user = fixture.Create<User>();
        userContextMock.Setup(x => x.UserId).Returns(user.Id);

        userRepositoryMock
            .Setup(x => x.GetById(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
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
        userContextMock.Setup(x => x.UserId).Returns(userId);
        
        var query = fixture.Create<GetLoggedInUserQuery>();

        var result = await handler.Handle(query, default);

        result.IsFailure.Should().BeTrue();
    }
}
