using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Shared;

namespace ExpenseSplitter.Api.Domain.ExpenseAllocations;

public sealed class ExpenseAllocation : Entity<ExpenseAllocationId>
{
    private ExpenseAllocation(
        Amount amount,
        ExpenseAllocationId id,
        ExpenseId expenseId,
        ParticipantId participantId
    ) : base(id)
    {
        ExpenseId = expenseId;
        ParticipantId = participantId;
        Amount = amount;
    }

    public ExpenseId ExpenseId { get; private set; }

    public ParticipantId ParticipantId { get; private set; }

    public Amount Amount { get; private set; }

    public static ExpenseAllocation Create(Amount amount, ExpenseId expenseId, ParticipantId participantId)
    {
        var expenseAllocation = new ExpenseAllocation(
            amount,
            ExpenseAllocationId.New(),
            expenseId,
            participantId
        );

        return expenseAllocation;
    }

    public void SetAmount(Amount amount)
    {
        Amount = amount;
    }

    public void SetParticipantId(ParticipantId participantId)
    {
        ParticipantId = participantId;
    }
}
