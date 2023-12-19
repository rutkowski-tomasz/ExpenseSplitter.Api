﻿using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Expenses.CreateExpense;

public sealed record CreateExpenseCommand(
    string Title,
    DateTime Date,
    Guid SettlementId,
    Guid PayingParticipantId,
    IEnumerable<CreateExpenseCommandAllocation> Allocations
) : ICommand<Guid>;

public sealed record CreateExpenseCommandAllocation(
    Guid ParticipantId,
    decimal Value
);