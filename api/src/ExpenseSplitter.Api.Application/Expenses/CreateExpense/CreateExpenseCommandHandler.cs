using ExpenseSplitter.Api.Application.Abstractions.Clock;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Expenses.CreateExpense;

public class CreateExpenseCommandHandler(
    ISettlementUserRepository settlementUserRepository,
    IExpenseRepository repository,
    ISettlementRepository settlementRepository,
    IDateTimeProvider timeProvider,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateExpenseCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var settlementId = new SettlementId(request.SettlementId);
        if (!await settlementUserRepository.CanUserAccessSettlement(settlementId, cancellationToken))
        {
            return Result.Failure<Guid>(SettlementErrors.Forbidden);
        }

        var settlement = await settlementRepository.GetById(settlementId, cancellationToken);

        if (!settlement!.AreAllParticipantsInSettlement([
            ..request.Allocations.Select(x => new ParticipantId(x.ParticipantId)),
            new ParticipantId(request.PayingParticipantId)
        ]))
        {
            return Result.Failure<Guid>(ParticipantErrors.NotFound);
        }

        settlement!.SetUpdatedOnUtc(timeProvider.UtcNow);
    
        var expenseResult = Expense.Create(
            request.Title,
            request.PaymentDate,
            new SettlementId(request.SettlementId), 
            new ParticipantId(request.PayingParticipantId),
            request.Allocations.ToDictionary(
                x => new ParticipantId(x.ParticipantId),
                x => x.Value
            )
        );

        if (expenseResult.IsFailure)
        {
            return Result.Failure<Guid>(expenseResult.AppError);
        }

        var expense = expenseResult.Value;
        repository.Add(expense);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return expenseResult.Value.Id.Value;
    }
}
