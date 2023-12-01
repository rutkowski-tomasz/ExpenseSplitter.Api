using ExpenseSplitter.Api.Domain.ExpenseAllocations;

namespace ExpenseSplitter.Api.Infrastructure.Repositories;

internal sealed class ExpenseAllocationRepository : Repository<ExpenseAllocation, ExpenseAllocationId>, IExpenseAllocationRepository
{
    public ExpenseAllocationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
