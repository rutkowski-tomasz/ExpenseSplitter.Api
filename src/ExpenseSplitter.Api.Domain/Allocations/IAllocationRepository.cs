using ExpenseSplitter.Api.Domain.Expenses;

namespace ExpenseSplitter.Api.Domain.Allocations;

public interface IAllocationRepository
{
    void Add(Allocation expenseAllocation);
    void Remove(Allocation expenseAllocation);
    Task<IEnumerable<Allocation>> GetAllByExpenseId(ExpenseId expenseId, CancellationToken cancellationToken);
}
