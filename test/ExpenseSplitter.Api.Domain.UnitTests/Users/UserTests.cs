using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Domain.Tests.Users;

public class UserTests
{
    [Fact]
    public void Create_ShouldReturnSuccess()
    {
        var nickname = new Fixture().Create<string>();

        var user = User.Create(nickname);

        user.IsSuccess.Should().BeTrue();
        user.Value.Nickname.Should().Be(nickname);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenNicknameIsEmpty()
    {
        var user = User.Create("");

        user.IsFailure.Should().BeTrue();
        user.Error.Code.Should().Be(UserErrors.EmptyNickname.Code);
    }
}

