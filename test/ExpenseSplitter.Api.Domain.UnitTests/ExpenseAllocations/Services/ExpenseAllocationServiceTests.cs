using ExpenseSplitter.Api.Domain.ExpenseAllocations;
using ExpenseSplitter.Api.Domain.ExpenseAllocations.Services;
using ExpenseSplitter.Api.Domain.Shared;

namespace ExpenseSplitter.Api.Domain.UnitTests.ExpenseAllocations.Services;

public class ExpenseAllocationServiceTests
{
    private readonly ExpenseAllocationService expenseAllocationService;

    public ExpenseAllocationServiceTests()
    {
        expenseAllocationService = new ExpenseAllocationService();
    }

    [Theory]
    [InlineData(10, ExpenseAllocationSplit.Amount, 5, 10, 10, 5)]
    [InlineData(15, ExpenseAllocationSplit.Part, 2, 4, 5, 5)]
    [InlineData(100, ExpenseAllocationSplit.Part, 1, 1, 10, 90)]
    [InlineData(100, ExpenseAllocationSplit.Part, 1, 1, 0, 100)]
    public void Calculate_ShouldReturnAmount_WhenDefinedExplicitly(
        decimal totalAmount,
        ExpenseAllocationSplit allocationSplit,
        decimal currentValue,
        decimal allPartsSum,
        decimal allAmountsSum,
        decimal expectedAmount
    )
    {
        var calculatedAmount = expenseAllocationService.Calculate(new Amount(totalAmount), allocationSplit, currentValue, allPartsSum, allAmountsSum);

        calculatedAmount.Should().NotBeNull();
        calculatedAmount.Value.Should().Be(expectedAmount);
    }
}