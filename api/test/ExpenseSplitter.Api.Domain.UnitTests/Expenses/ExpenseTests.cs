using ExpenseSplitter.Api.Domain.Common;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Domain.UnitTests.Expenses;

public class ExpenseTests
{
    private readonly Fixture fixture = new();

    [Fact]
    public void Create_ShouldReturnSuccess()
    {
        var title = fixture.Create<string>();
        var dateTime = fixture.Create<DateTime>();
        var settlementId = fixture.Create<SettlementId>();
        var participantIds = fixture.CreateMany<ParticipantId>().ToList();
        var allocations = participantIds.ToDictionary(
            x => x,
            _ => fixture.Create<decimal>()
        );

        var expense = Expense.Create(
            title,
            DateOnly.FromDateTime(dateTime),
            settlementId,
            participantIds[0],
            allocations
        );

        var expectedTotalAmount = Amount.Create(allocations.Sum(x => x.Value)).Value;

        expense.IsSuccess.Should().BeTrue();
        expense.Value.Id.Value.Should().NotBeEmpty();
        expense.Value.Title.Should().Be(title);
        expense.Value.Amount.Should().Be(expectedTotalAmount);
        expense.Value.PaymentDate.Should().Be(DateOnly.FromDateTime(dateTime));
        expense.Value.SettlementId.Should().Be(settlementId);
        expense.Value.PayingParticipantId.Should().Be(participantIds[0]);
    }

    [Fact]
    public void ExpenseIdNew_ShouldGenerateNonEmptyGuid()
    {
        ExpenseId.New().Value.Should().NotBeEmpty();
    }
}
