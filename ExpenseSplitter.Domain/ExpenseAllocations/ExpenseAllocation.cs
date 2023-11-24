using ExpenseSplitter.Domain.Expenses;
using ExpenseSplitter.Domain.Participants;

namespace ExpenseSplitter.Domain.ExpenseAllocations;

public class ExpenseAllocation
{
    private ExpenseAllocation()
    {

    }

    public ExpenseAllocationId Id { get; private set; }

    public ExpenseId ExpenseId { get; private set; }
    public ParticipantId ParticipantId { get; set; }

    public decimal Value { get; private set; }

    public static ExpenseAllocation Create(ExpenseId expenseId, ParticipantId participantId, decimal value)
    {
        var expenseAllocation = new ExpenseAllocation()
        {
            Id = new ExpenseAllocationId(Guid.NewGuid()),
            ExpenseId = expenseId,
            ParticipantId = participantId,
            Value = value
        };

        return expenseAllocation;
    }
}
