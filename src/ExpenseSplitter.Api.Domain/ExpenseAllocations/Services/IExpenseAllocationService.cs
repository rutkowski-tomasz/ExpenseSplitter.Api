using ExpenseSplitter.Api.Domain.Shared;

namespace ExpenseSplitter.Api.Domain.ExpenseAllocations.Services;

public interface IExpenseAllocationService
{
    Amount Calculate(
        Amount totalAmount,
        ExpenseAllocationSplit allocationSplit,
        decimal currentValue,
        decimal allPartsSum,
        decimal allAmountsSum
    );
}