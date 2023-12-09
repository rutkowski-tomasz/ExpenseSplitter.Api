using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Domain.Expenses;

public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(ExpenseId expenseId, CancellationToken cancellationToken = default);

    void Add(Expense expense);
    
    Task<IEnumerable<Expense>> GetAllWithSettlementId(SettlementId settlementId, int page, int pageSize, CancellationToken cancellationToken);

    void Remove(Expense expense);
}
