using ExpenseSplitter.Domain.Expenses;
using ExpenseSplitter.Domain.Participants;
using ExpenseSplitter.Domain.Settlements;

namespace ExpenseSplitter.Domain.Tests.Expenses;

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
    }
}
