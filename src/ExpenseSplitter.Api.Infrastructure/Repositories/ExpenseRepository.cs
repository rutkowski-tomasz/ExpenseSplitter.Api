using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Settlements;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.Repositories;

internal sealed class ExpenseRepository : Repository<Expense, ExpenseId>, IExpenseRepository
{
    public ExpenseRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Expense>> GetAllBySettlementId(
        SettlementId settlementId,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var skip = (page - 1) * pageSize;

        return await DbContext
            .Set<Expense>()
            .Where(x => x.SettlementId == settlementId)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
