using System.Net.Mail;
using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Users.RegisterUser;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Application.UnitTests.Users;

public class RegisterUserCommandHandlerTests
{
    private readonly RegisterUserCommandHandler handler;
    private readonly Fixture fixture = CustomFixture.Create();
    private readonly IAuthenticationService authenticationService = Substitute.For<IAuthenticationService>();
    private readonly IUserRepository userRepository = Substitute.For<IUserRepository>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();

    public RegisterUserCommandHandlerTests()
    {
        handler = new RegisterUserCommandHandler(
            authenticationService,
            userRepository,
            unitOfWork
        );
    }

    [Fact]
    public async Task Handle_ShouldSuccess()
    {
        var command = fixture
            .Build<RegisterUserCommand>()
            .With(x => x.Email, fixture.Create<MailAddress>().Address)
            .Create();

        authenticationService
            .RegisterAsync(command.Email, command.Password, Arg.Any<CancellationToken>())
            .Returns(Guid.CreateVersion7().ToString());

        var result = await handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenNicknameIsEmpty()
    {
        var command = fixture
            .Build<RegisterUserCommand>()
            .With(x => x.Email, fixture.Create<MailAddress>().Address)
            .With(x => x.Nickname, string.Empty)
            .Create();

        var identityId = Guid.CreateVersion7();

        authenticationService
            .RegisterAsync(command.Email, command.Password, Arg.Any<CancellationToken>())
            .Returns(identityId.ToString());

        var result = await handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.AppError.Should().Be(UserErrors.EmptyNickname);
    }
}
