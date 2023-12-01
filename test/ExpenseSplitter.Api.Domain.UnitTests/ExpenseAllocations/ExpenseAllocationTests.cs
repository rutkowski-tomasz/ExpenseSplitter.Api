using ExpenseSplitter.Api.Domain.ExpenseAllocations;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Shared;

namespace ExpenseSplitter.Api.Domain.UnitTests.ExpenseAllocations;

public class ExpenseAllocationTests
{
    [Fact]
    public void Create_ShouldReturnSuccess()
    {
        var amount = new Fixture().Create<Amount>();
        var expenseId = new Fixture().Create<ExpenseId>();
        var participantId = new Fixture().Create<ParticipantId>();

        var expenseAllocation = ExpenseAllocation.Create(amount, expenseId, participantId);
        
        expenseAllocation.Should().NotBeNull();
        expenseAllocation.Id.Value.Should().NotBeEmpty();
        expenseAllocation.Amount.Should().Be(amount);
        expenseAllocation.ExpenseId.Should().Be(expenseId);
        expenseAllocation.ParticipantId.Should().Be(participantId);
    }

    [Fact]
    public void ExpenseAllocationIdNew_ShouldGenerateNonEmptyGuid()
    {
        ExpenseAllocationId.New().Value.Should().NotBeEmpty();
    }
}

