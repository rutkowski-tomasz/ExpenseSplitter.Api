using ExpenseSplitter.Api.Domain.Shared;

namespace ExpenseSplitter.Api.Domain.UnitTests.Shared;

public class AmountTests
{
    [Fact]
    public void IsZero_ShouldReturnTrue_WhenValueIsZero()
    {
        var amount = new Amount(0);

        amount.IsZero().Should().BeTrue();
    }

    [Fact]
    public void IsZero_ShouldReturnTrue_WhenZeroCreated()
    {
        var amount = Amount.Zero();

        amount.IsZero().Should().BeTrue();
    }
}