using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.Shared;

namespace ExpenseSplitter.Api.Domain.UnitTests.Expenses;

public class ExpenseTests
{
    [Fact]
    public void Create_ShouldReturnSuccess()
    {
        var name = new Fixture().Create<string>();
        var amount = new Fixture().Create<Amount>();
        var dateTime = new Fixture().Create<DateTime>();
        var settlementId = new Fixture().Create<SettlementId>();
        var participantId = new Fixture().Create<ParticipantId>();

        var expense = Expense.Create(name, amount, DateOnly.FromDateTime(dateTime), settlementId, participantId);
        
        expense.IsSuccess.Should().BeTrue();
        expense.Value.Id.Value.Should().NotBeEmpty();
        expense.Value.Title.Should().Be(name);
        expense.Value.Amount.Should().Be(amount);
        expense.Value.PaymentDate.Should().Be(DateOnly.FromDateTime(dateTime));
        expense.Value.SettlementId.Should().Be(settlementId);
        expense.Value.PayingParticipantId.Should().Be(participantId);
    }

    [Fact]
    public void ExpenseIdNew_ShouldGenerateNonEmptyGuid()
    {
        ExpenseId.New().Value.Should().NotBeEmpty();
    }
}
