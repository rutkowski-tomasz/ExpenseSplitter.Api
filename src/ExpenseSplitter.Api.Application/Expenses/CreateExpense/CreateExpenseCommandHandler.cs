using ExpenseSplitter.Api.Application.Abstractions.Clock;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Common;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Expenses.CreateExpense;

public class CreateExpenseCommandHandler(
    ISettlementUserRepository userRepository,
    IExpenseRepository repository,
    IAllocationRepository allocationRepository,
    IParticipantRepository participantRepository,
    ISettlementRepository settlementRepository,
    IDateTimeProvider timeProvider,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateExpenseCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var settlementId = new SettlementId(request.SettlementId);
        if (!await userRepository.CanUserAccessSettlement(settlementId, cancellationToken))
        {
            return Result.Failure<Guid>(SettlementErrors.Forbidden);
        }

        if (!await AreParticipantIdsValid(request, cancellationToken))
        {
            return Result.Failure<Guid>(ParticipantErrors.NotFound);
        }

        var settlement = await settlementRepository.GetById(settlementId, cancellationToken);
        settlement!.SetUpdatedOnUtc(timeProvider.UtcNow);

        var totalAmountResult = Amount.Create(request.Allocations.Sum(x => x.Value));
        if (totalAmountResult.IsFailure)
        {
            return Result.Failure<Guid>(totalAmountResult.Error);
        }
    
        var expenseResult = Expense.Create(
            request.Title,
            totalAmountResult.Value,
            request.PaymentDate,
            new SettlementId(request.SettlementId), 
            new ParticipantId(request.PayingParticipantId)
        );

        if (expenseResult.IsFailure)
        {
            return Result.Failure<Guid>(expenseResult.Error);
        }

        var expense = expenseResult.Value;
        repository.Add(expense);

        var allocations = CreateAllocations(request, expense.Id);

        foreach (var allocation in allocations)
        {
            allocationRepository.Add(allocation);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return expenseResult.Value.Id.Value;
    }
    
    private Task<bool> AreParticipantIdsValid(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var participantIds = request
            .Allocations
            .Select(x => new ParticipantId(x.ParticipantId))
            .ToList();
        
        participantIds.Add(new ParticipantId(request.PayingParticipantId));

        return participantRepository.AreAllParticipantsInSettlement(
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
                Amount.Create(x.Value).Value,
                expenseId,
                new ParticipantId(x.ParticipantId)
            ));

        return allocations;
    }
}
