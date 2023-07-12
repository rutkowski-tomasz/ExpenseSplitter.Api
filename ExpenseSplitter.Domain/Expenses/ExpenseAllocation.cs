﻿using ExpenseSplitter.Domain.Settlements;

namespace ExpenseSplitter.Domain.Expenses;

public class ExpenseAllocation
{
    private ExpenseAllocation()
    {

    }

    public ExpenseAllocationId Id { get; private set; }

    public ExpenseId ExpenseId { get; private set; }

    public decimal Value { get; private set; }

    private readonly HashSet<ExpenseAllocationParticipant> expenseAllocationParticipants = new();

    public static ExpenseAllocation Create(ExpenseId expenseId, decimal value)
    {
        var expenseAllocation = new ExpenseAllocation()
        {
            Id = new ExpenseAllocationId(Guid.NewGuid()),
            ExpenseId = expenseId,
            Value = value,
        };

        return expenseAllocation;
    }

    public void AddExpenseAllocationPartipant(ParticipantId participantId, decimal part)
    {
        var expenseAllocationParticipant = ExpenseAllocationParticipant.Create(Id, participantId, part);

        expenseAllocationParticipants.Add(expenseAllocationParticipant);
    }
}
