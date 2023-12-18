﻿using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;
using ExpenseSplitter.Api.Domain.Shared;

namespace ExpenseSplitter.Api.Application.Expenses.CreateExpense;

public class CreateExpenseCommandHandler : ICommandHandler<CreateExpenseCommand, Guid>
{
    private readonly ISettlementUserRepository settlementUserRepository;
    private readonly IExpenseRepository expenseRepository;
    private readonly IAllocationRepository allocationRepository;
    private readonly IParticipantRepository participantRepository;
    private readonly IUnitOfWork unitOfWork;

    public CreateExpenseCommandHandler(
        ISettlementUserRepository settlementUserRepository,
        IExpenseRepository expenseRepository,
        IAllocationRepository allocationRepository,
        IParticipantRepository participantRepository,
        IUnitOfWork unitOfWork
    )
    {
        this.settlementUserRepository = settlementUserRepository;
        this.expenseRepository = expenseRepository;
        this.allocationRepository = allocationRepository;
        this.participantRepository = participantRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        if (!await settlementUserRepository.CanUserAccessSettlement(new SettlementId(request.SettlementId), cancellationToken))
        {
            return Result.Failure<Guid>(SettlementErrors.Forbidden);
        }

        if (!await AreParticipantIdsValid(request, cancellationToken))
        {
            return Result.Failure<Guid>(ParticipantErrors.NotFound);
        }

        var totalAmount = request.Allocations.Sum(x => x.Value);
        var expenseResult = Expense.Create(
            request.Title,
            new Amount(totalAmount),
            request.Date,
            new SettlementId(request.SettlementId), 
            new ParticipantId(request.PayingParticipantId)
        );

        if (expenseResult.IsFailure)
        {
            return Result.Failure<Guid>(expenseResult.Error);
        }

        var expense = expenseResult.Value;
        expenseRepository.Add(expense);

        var allocations = CreateAllocations(request, expense.Id);

        foreach (var allocation in allocations)
        {
            allocationRepository.Add(allocation);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return expenseResult.Value.Id.Value;
    }
    
    private async Task<bool> AreParticipantIdsValid(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var participantIds = request
            .Allocations
            .Select(x => new ParticipantId(x.ParticipantId))
            .ToList();
        
        participantIds.Add(new ParticipantId(request.PayingParticipantId));

        return await participantRepository.AreAllParticipantsInSettlement(
            new SettlementId(request.SettlementId),
            participantIds,
            cancellationToken
        );
    }

    private static IEnumerable<Allocation> CreateAllocations(CreateExpenseCommand request, ExpenseId expenseId)
    {
        var allocations = request
            .Allocations
            .Select(x => Allocation.Create(
                new Amount(x.Value),
                expenseId,
                new ParticipantId(x.ParticipantId)
            ));

        return allocations;
    }
}
