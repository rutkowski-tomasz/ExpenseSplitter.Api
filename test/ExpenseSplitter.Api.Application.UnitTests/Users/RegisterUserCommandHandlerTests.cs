using System.Net.Mail;
using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Users.RegisterUser;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Application.UnitTests.Users;

public class RegisterUserCommandHandlerTests
{
    private readonly RegisterUserCommandHandler handler;
    private readonly Mock<IAuthenticationService> authenticationServiceMock;

    public RegisterUserCommandHandlerTests()
    {
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
        var command = new Fixture()
            .Build<RegisterUserCommand>()
            .With(x => x.Email, new Fixture().Create<MailAddress>().Address)
            .Create();

        authenticationServiceMock
            .Setup(x => x.RegisterAsync(It.Is<User>(y => y.Email == command.Email), command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid().ToString());

        var result = await handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenNicknameIsEmpty()
    {
        var command = new Fixture()
            .Build<RegisterUserCommand>()
            .With(x => x.Email, new Fixture().Create<MailAddress>().Address)
            .With(x => x.Nickname, string.Empty)
            .Create();

        var identityId = Guid.NewGuid();

        authenticationServiceMock
            .Setup(x => x.RegisterAsync(It.Is<User>(y => y.Email == command.Email), command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(identityId.ToString());

        var result = await handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.EmptyNickname);
    }
}
