using System.Net.Mail;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Domain.UnitTests.Users;

public class UserTests
{
    [Fact]
    public void Create_ShouldReturnSuccess()
    {
        var email = new Fixture().Create<MailAddress>().Address;
        var nickname = new Fixture().Create<string>();

        var user = User.Create(nickname, email, UserId.New());

        user.IsSuccess.Should().BeTrue();
        user.Value.Nickname.Should().Be(nickname);
        user.Value.Email.Should().Be(email);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenNicknameIsEmpty()
    {
        var email = new Fixture().Create<MailAddress>().Address;

        var user = User.Create("", email, UserId.New());

        user.IsFailure.Should().BeTrue();
        user.AppError.Type.Should().Be(UserErrors.EmptyNickname.Type);
    }

    [Fact]
    public void UserIdNew_ShouldGenerateNonEmptyGuid()
    {
        UserId.New().Value.Should().NotBeEmpty();
    }
}
