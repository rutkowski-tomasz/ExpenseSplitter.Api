namespace ExpenseSplitter.Domain.Expenses;

public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(ExpenseId expenseId, CancellationToken cancellationToken = default);
}
