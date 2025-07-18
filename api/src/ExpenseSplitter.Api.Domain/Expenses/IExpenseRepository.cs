using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Domain.Expenses;

public interface IExpenseRepository
{
    void Add(Expense expense);
    void Remove(Expense expense);
    Task<Expense?> GetById(ExpenseId expenseId, CancellationToken cancellationToken);
    Task<IEnumerable<Expense>> GetPagedBySettlementId(SettlementId settlementId, int page, int pageSize, CancellationToken cancellationToken);
    Task<IEnumerable<Expense>> GetAllBySettlementId(SettlementId settlementId, CancellationToken cancellationToken);
}
