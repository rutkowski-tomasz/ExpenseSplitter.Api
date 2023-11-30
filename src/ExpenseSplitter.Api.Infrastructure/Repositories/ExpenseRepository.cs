using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Settlements;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.Repositories;

internal sealed class ExpenseRepository : Repository<Expense, ExpenseId>, IExpenseRepository
{
    public ExpenseRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Expense>> GetAllWithSettlementId(
        SettlementId settlementId,
        CancellationToken cancellationToken
    )
    {
        return await DbContext
            .Set<Expense>()
            .Where(x => x.SettlementId == settlementId)
            .ToListAsync(cancellationToken);
    }
}
