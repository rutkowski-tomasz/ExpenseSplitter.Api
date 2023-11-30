using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Domain.Expenses;

public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(ExpenseId expenseId, CancellationToken cancellationToken = default);

    void Add(Expense expense);
    
    Task<IEnumerable<Expense>> GetAllWithSettlementId(SettlementId settlementId, CancellationToken cancellationToken);
}
