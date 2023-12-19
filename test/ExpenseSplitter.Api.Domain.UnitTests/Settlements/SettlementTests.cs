using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Domain.UnitTests.Settlements;

public class SettlementTests
{
    [Fact]
    public void Create_ShouldReturnSuccess()
    {
        var name = new Fixture().Create<string>();
        var inviteCode = new Fixture().Create<string>();
        var creatorUserId = new Fixture().Create<UserId>();
        var createdOnUtc = DateTime.UtcNow;

        var settlement = Settlement.Create(name, inviteCode, creatorUserId, createdOnUtc);

        settlement.IsSuccess.Should().BeTrue();
        settlement.Value.Id.Value.Should().NotBeEmpty();
        settlement.Value.Name.Should().Be(name);
        settlement.Value.InviteCode.Should().Be(inviteCode);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenNameIsEmpty()
    {
        var creatorUserId = new Fixture().Create<UserId>();

        var settlement = Settlement.Create("", "", creatorUserId, DateTime.UtcNow);

        settlement.IsFailure.Should().BeTrue();
        settlement.Error.Code.Should().Be(SettlementErrors.EmptyName.Code);
    }

    [Fact]
    public void SettlementIdNew_ShouldGenerateNonEmptyGuid()
    {
        SettlementId.New().Value.Should().NotBeEmpty();
    }
}
