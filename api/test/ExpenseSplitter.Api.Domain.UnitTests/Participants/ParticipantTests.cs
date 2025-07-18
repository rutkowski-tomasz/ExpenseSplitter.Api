using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Domain.UnitTests.Participants;

public class ParticipantTests
{
    [Fact]
    public void Create_ShouldReturnSuccess()
    {
        var nickname = new Fixture().Create<string>();
        var settlementId = new Fixture().Create<SettlementId>();

        var participant = Participant.Create(settlementId, nickname);

        participant.IsSuccess.Should().BeTrue();
        participant.Value.Id.Value.Should().NotBeEmpty();
        participant.Value.Nickname.Should().Be(nickname);
        participant.Value.SettlementId.Should().Be(settlementId);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenNicknameIsEmpty()
    {
        var settlementId = new Fixture().Create<SettlementId>();

        var participant = Participant.Create(settlementId, "");

        participant.IsFailure.Should().BeTrue();
        participant.AppError.Type.Should().Be(ParticipantErrors.NicknameEmpty.Type);
    }

    [Fact]
    public void ParticipantIdNew_ShouldGenerateNonEmptyGuid()
    {
        ParticipantId.New().Value.Should().NotBeEmpty();
    }
}
