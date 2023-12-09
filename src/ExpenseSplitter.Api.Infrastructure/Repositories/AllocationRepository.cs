using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Expenses;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.Repositories;

internal sealed class AllocationRepository : Repository<Allocation, AllocationId>, IAllocationRepository
{
    public AllocationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Allocation>> GetAllWithExpenseId(ExpenseId expenseId, CancellationToken cancellationToken)
    {
        return await DbContext
            .Set<Allocation>()
            .Where(x => x.ExpenseId == expenseId)
            .ToListAsync(cancellationToken);
    }

    public void Remove(Allocation expenseAllocation)
    {
        DbContext.Set<Allocation>().Remove(expenseAllocation);
    }

    public async Task<Allocation?> GetWithId(AllocationId expenseAllocationId, CancellationToken cancellationToken)
    {
        return await DbContext
            .Set<Allocation>()
            .FirstOrDefaultAsync(x => x.Id == expenseAllocationId);
    }
}
