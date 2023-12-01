using ExpenseSplitter.Api.Domain.Shared;

namespace ExpenseSplitter.Api.Domain.ExpenseAllocations.Services;

public class ExpenseAllocationService : IExpenseAllocationService
{
    public Amount Calculate(
        Amount totalAmount,
        ExpenseAllocationSplit allocationSplit,
        decimal currentValue,
        decimal allPartsSum,
        decimal allAmountsSum
    )
    {
        return allocationSplit == ExpenseAllocationSplit.Amount
            ? new Amount(currentValue)
            : new Amount((totalAmount.Value - allAmountsSum) * currentValue / allPartsSum);
    }
}
