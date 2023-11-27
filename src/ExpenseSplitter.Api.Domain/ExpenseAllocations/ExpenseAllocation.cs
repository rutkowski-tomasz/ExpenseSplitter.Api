using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;

namespace ExpenseSplitter.Api.Domain.ExpenseAllocations;

public sealed class ExpenseAllocation : Entity<ExpenseAllocationId>
{
    private ExpenseAllocation(
        ExpenseAllocationId id,
        ExpenseId expenseId,
        ParticipantId participantId,
        decimal value
    ) : base(id)
    {
        ExpenseId = expenseId;
        ParticipantId = participantId;
        Value = value;
    }

    public ExpenseId ExpenseId { get; private set; }

    public ParticipantId ParticipantId { get; private set; }

    public decimal Value { get; private set; }

    public static ExpenseAllocation Create(ExpenseId expenseId, ParticipantId participantId, decimal value)
    {
        var expenseAllocation = new ExpenseAllocation(
            ExpenseAllocationId.New(),
            expenseId,
            participantId,
            value
        );

        return expenseAllocation;
    }
}
