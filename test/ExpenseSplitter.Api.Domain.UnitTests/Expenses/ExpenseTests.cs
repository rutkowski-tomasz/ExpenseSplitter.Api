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

        var expense = Expense.Create(settlementId, name, participantId);

        expense.Should().NotBeNull();
        expense.Name.Should().Be(name);
        expense.SettlementId.Should().Be(settlementId);
        expense.PayingParticipantId.Should().Be(participantId);
    }
}
