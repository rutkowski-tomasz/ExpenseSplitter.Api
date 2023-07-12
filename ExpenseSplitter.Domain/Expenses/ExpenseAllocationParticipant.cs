﻿using ExpenseSplitter.Domain.Settlements;

namespace ExpenseSplitter.Domain.Expenses;

public class ExpenseAllocationParticipant
{
	private ExpenseAllocationParticipant()
	{

	}

    public ExpenseAllocationId ExpenseAllocationId { get; private set; }

	public ParticipantId ParticipantId { get; private set; }

	public decimal Part { get; private set; }

	public static ExpenseAllocationParticipant Create(
		ExpenseAllocationId expenseAllocationId,
		ParticipantId participantId,
		decimal part
	)
	{
		if (part <= 0)
		{
			throw new ArgumentException($"{nameof(part)} must be a positive number");
		}

		var expenseAllocationParticipant = new ExpenseAllocationParticipant()
		{
			ExpenseAllocationId = expenseAllocationId,
			ParticipantId = participantId,
			Part = part,
		};

		return expenseAllocationParticipant;
    }
}
