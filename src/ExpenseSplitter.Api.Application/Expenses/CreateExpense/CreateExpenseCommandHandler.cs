using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Application.Expenses.CreateExpense;

public class CreateExpenseCommandHandler : ICommandHandler<CreateExpenseCommand, Guid>
{
    private readonly IExpenseRepository expenseRepository;
    private readonly IUnitOfWork unitOfWork;

    public CreateExpenseCommandHandler(
        IExpenseRepository expenseRepository,
        IUnitOfWork unitOfWork
    )
    {
        this.expenseRepository = expenseRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var result = Expense.Create(
            request.Name,
            new SettlementId(request.SettlementId), 
            new ParticipantId(request.PayingParticipantId)
        );

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }
        
        expenseRepository.Add(result.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return result.Value.Id.Value;
    }
}
