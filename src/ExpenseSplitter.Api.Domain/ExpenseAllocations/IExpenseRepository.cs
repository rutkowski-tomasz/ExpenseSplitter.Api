namespace ExpenseSplitter.Api.Domain.ExpenseAllocations;

public interface IExpenseAllocationRepository
{
    void Add(ExpenseAllocation expenseAllocation);
}
