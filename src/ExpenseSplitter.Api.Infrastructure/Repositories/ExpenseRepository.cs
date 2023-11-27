using ExpenseSplitter.Api.Domain.Expenses;

namespace ExpenseSplitter.Api.Infrastructure.Repositories;

internal sealed class ExpenseRepository : Repository<Expense, ExpenseId>, IExpenseRepository
{
    public ExpenseRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
