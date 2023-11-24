using ExpenseSplitter.Domain.Expenses;

namespace ExpenseSplitter.Infrastructure.Repositories;

internal sealed class ExpenseRepository : Repository<Expense, ExpenseId>, IExpenseRepository
{
    public ExpenseRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
