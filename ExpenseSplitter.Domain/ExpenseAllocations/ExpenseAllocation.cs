﻿using ExpenseSplitter.Domain.Abstractions;
using ExpenseSplitter.Domain.Expenses;
using ExpenseSplitter.Domain.Participants;

namespace ExpenseSplitter.Domain.ExpenseAllocations;

public class ExpenseAllocation : Entity<ExpenseAllocationId>
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
