using ExpenseSplitter.Api.Domain.ExpenseAllocations;
using ExpenseSplitter.Api.Domain.Expenses;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.Repositories;

internal sealed class ExpenseAllocationRepository : Repository<ExpenseAllocation, ExpenseAllocationId>, IExpenseAllocationRepository
{
    public ExpenseAllocationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<ExpenseAllocation>> GetAllWithExpenseId(ExpenseId expenseId, CancellationToken cancellationToken)
    {
        return await DbContext
            .Set<ExpenseAllocation>()
            .Where(x => x.ExpenseId == expenseId)
            .ToListAsync(cancellationToken);
    }

    public void Remove(ExpenseAllocation expenseAllocation)
    {
        DbContext.Set<ExpenseAllocation>().Remove(expenseAllocation);
    }

    public async Task<ExpenseAllocation?> GetWithId(ExpenseAllocationId expenseAllocationId, CancellationToken cancellationToken)
    {
        return await DbContext
            .Set<ExpenseAllocation>()
            .FirstOrDefaultAsync(x => x.Id == expenseAllocationId);
    }
}
