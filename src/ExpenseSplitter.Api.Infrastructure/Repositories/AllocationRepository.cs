using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Expenses;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.Repositories;

internal sealed class AllocationRepository : Repository<Allocation, AllocationId>, IAllocationRepository
{
    public AllocationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Allocation>> GetAllByExpenseId(ExpenseId expenseId, CancellationToken cancellationToken)
    {
        return await DbContext
            .Set<Allocation>()
            .Where(x => x.ExpenseId == expenseId)
            .ToListAsync(cancellationToken);
    }
}
