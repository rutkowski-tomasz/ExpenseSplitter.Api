using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Abstractions.Metrics;
using ExpenseSplitter.Api.Application.Users.LoginUser;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Application.UnitTests.Users;

public class LoginUserCommandHandlerTests
{
    private readonly LoginUserCommandHandler handler;
    private readonly Fixture fixture = CustomFixture.Create();
    private readonly IJwtService jwtService = Substitute.For<IJwtService>();
    private readonly IMetricsService metricsService = Substitute.For<IMetricsService>();

    public LoginUserCommandHandlerTests()
    {
        handler = new LoginUserCommandHandler(jwtService, metricsService);
    }

    [Fact]
    public async Task Handler_ShouldReturnAccessToken()
    {
        jwtService
            .GetAccessTokenAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("access-token");

        var command = fixture.Create<LoginUserCommand>();

        var result = await handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("access-token");
    }

    [Fact]
    public async Task Handler_ShouldFail_WhenGettingAccessTokenResultsInFailure()
    {
        jwtService
            .GetAccessTokenAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<string>(UserErrors.InvalidCredentials));

        var command = fixture.Create<LoginUserCommand>();

        var result = await handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.AppError.Should().Be(UserErrors.InvalidCredentials);
    }
}
