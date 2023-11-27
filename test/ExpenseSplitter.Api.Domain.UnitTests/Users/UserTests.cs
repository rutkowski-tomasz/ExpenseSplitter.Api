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

        var user = User.Create(nickname, email);

        user.IsSuccess.Should().BeTrue();
        user.Value.Nickname.Should().Be(nickname);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenNicknameIsEmpty()
    {
        var email = new Fixture().Create<MailAddress>().Address;

        var user = User.Create("", email);

        user.IsFailure.Should().BeTrue();
        user.Error.Code.Should().Be(UserErrors.EmptyNickname.Code);
    }
}

