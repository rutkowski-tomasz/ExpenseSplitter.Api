using System.Net.Mail;
using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Users.RegisterUser;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Application.UnitTests.Users;

public class RegisterUserCommandHandlerTests
{
    private readonly RegisterUserCommandHandler handler;
    private readonly Fixture fixture;
    private readonly Mock<IAuthenticationService> authenticationServiceMock;

    public RegisterUserCommandHandlerTests()
    {
        fixture = CustomFixture.Create();
        authenticationServiceMock = new Mock<IAuthenticationService>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        handler = new RegisterUserCommandHandler(
            authenticationServiceMock.Object,
            userRepositoryMock.Object,
            unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldSuccess()
    {
        var command = fixture
            .Build<RegisterUserCommand>()
            .With(x => x.Email, fixture.Create<MailAddress>().Address)
            .Create();

        authenticationServiceMock
            .Setup(x => x.RegisterAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.CreateVersion7().ToString());

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

        authenticationServiceMock
            .Setup(x => x.RegisterAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(identityId.ToString());

        var result = await handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.AppError.Should().Be(UserErrors.EmptyNickname);
    }
}
