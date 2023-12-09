using ExpenseSplitter.Api.Domain.Expenses;

namespace ExpenseSplitter.Api.Domain.Allocations;

public interface IAllocationRepository
{
    void Add(Allocation expenseAllocation);

    Task<IEnumerable<Allocation>> GetAllWithExpenseId(ExpenseId expenseId, CancellationToken cancellationToken);

    void Remove(Allocation expenseAllocation);

    Task<Allocation?> GetWithId(AllocationId expenseAllocationId, CancellationToken cancellationToken);
}
