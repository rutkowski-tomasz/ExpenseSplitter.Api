using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Domain.UnitTests.Participants;

public class ParticipantTests
{
    [Fact]
    public void Create_ShouldReturnSuccess()
    {
        var nickname = new Fixture().Create<string>();
        var settlementId = new Fixture().Create<SettlementId>();
        var userId = new Fixture().Create<UserId>();

        var participant = Participant.Create(settlementId, userId, nickname);

        participant.IsSuccess.Should().BeTrue();
        participant.Value.Id.Value.Should().NotBeEmpty();
        participant.Value.Nickname.Should().Be(nickname);
        participant.Value.SettlementId.Should().Be(settlementId);
        participant.Value.UserId.Should().Be(userId);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenNicknameIsEmpty()
    {
        var settlementId = new Fixture().Create<SettlementId>();
        var userId = new Fixture().Create<UserId>();

        var participant = Participant.Create(settlementId, userId, "");

        participant.IsFailure.Should().BeTrue();
        participant.Error.Code.Should().Be(ParticipantErrors.NicknameEmpty.Code);
    }

    [Fact]
    public void ParticipantIdNew_ShouldGenerateNonEmptyGuid()
    {
        ParticipantId.New().Value.Should().NotBeEmpty();
    }
}
