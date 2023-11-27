using ExpenseSplitter.Api.Domain.ExpenseAllocations;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;

namespace ExpenseSplitter.Api.Domain.UnitTests.ExpenseAllocations;

public class ExpenseAllocationTests
{
    [Fact]
    public void Create_ShouldReturnSuccess()
    {
        var value = new Fixture().Create<decimal>();
        var expenseId = new Fixture().Create<ExpenseId>();
        var participantId = new Fixture().Create<ParticipantId>();

        var expenseAllocation = ExpenseAllocation.Create(expenseId, participantId, value);

        expenseAllocation.Should().NotBeNull();
        expenseAllocation.Value.Should().Be(value);
        expenseAllocation.ExpenseId.Should().Be(expenseId);
        expenseAllocation.ParticipantId.Should().Be(participantId);
    }
}

