using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Users.LoginUser;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Application.UnitTests.Users;

public class LoginUserCommandHandlerTests
{
    private readonly LoginUserCommandHandler handler;
    private readonly Mock<IJwtService> jwtServiceMock;

    public LoginUserCommandHandlerTests()
    {
        jwtServiceMock = new Mock<IJwtService>();

        handler = new LoginUserCommandHandler(jwtServiceMock.Object);
    }

    [Fact]
    public async Task Handler_ShouldReturnAccessToken()
    {
        jwtServiceMock
            .Setup(x => x.GetAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("access-token");

        var command = new Fixture().Create<LoginUserCommand>();

        var result = await handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("access-token");
    }

    [Fact]
    public async Task Handler_ShouldFail_WhenGettingAccessTokenResultsInFailure()
    {
        jwtServiceMock
            .Setup(x => x.GetAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<string>(UserErrors.InvalidCredentials));

        var command = new Fixture().Create<LoginUserCommand>();

        var result = await handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidCredentials);
    }
}