using ExpenseSplitter.Api.Domain.Expenses;

namespace ExpenseSplitter.Api.Domain.ExpenseAllocations;

public interface IExpenseAllocationRepository
{
    void Add(ExpenseAllocation expenseAllocation);

    Task<IEnumerable<ExpenseAllocation>> GetAllWithExpenseId(ExpenseId expenseId, CancellationToken cancellationToken);

    void Remove(ExpenseAllocation expenseAllocation);

    Task<ExpenseAllocation?> GetWithId(ExpenseAllocationId expenseAllocationId, CancellationToken cancellationToken);
}
