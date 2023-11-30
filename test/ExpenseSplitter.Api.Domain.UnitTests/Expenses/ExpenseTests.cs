using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Domain.UnitTests.Expenses;

public class ExpenseTests
{
    [Fact]
    public void Create_ShouldReturnSuccess()
    {
        var name = new Fixture().Create<string>();
        var settlementId = new Fixture().Create<SettlementId>();
        var participantId = new Fixture().Create<ParticipantId>();

        var expense = Expense.Create(name, settlementId, participantId);

        expense.IsSuccess.Should().BeTrue();
        expense.Value.Name.Should().Be(name);
        expense.Value.SettlementId.Should().Be(settlementId);
        expense.Value.PayingParticipantId.Should().Be(participantId);
    }
}
