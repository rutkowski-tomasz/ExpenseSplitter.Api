using ExpenseSplitter.Domain.Settlements;

namespace ExpenseSplitter.Domain.Tests.Settlements;

public class SettlementTests
{
    [Fact]
    public void Create_ShouldReturnSuccess()
    {
        var name = new Fixture().Create<string>();

        var settlement = Settlement.Create(name);

        settlement.IsSuccess.Should().BeTrue();
        settlement.Value.Name.Should().Be(name);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenNameIsEmpty()
    {
        var settlement = Settlement.Create("");

        settlement.IsFailure.Should().BeTrue();
        settlement.Error.Code.Should().Be(SettlementErrors.EmptyName.Code);
    }
}
