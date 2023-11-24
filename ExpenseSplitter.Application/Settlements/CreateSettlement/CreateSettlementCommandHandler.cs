using ExpenseSplitter.Application.Abstractions.Cqrs;
using ExpenseSplitter.Domain.Abstractions;
using ExpenseSplitter.Domain.Settlements;

namespace ExpenseSplitter.Application.Settlements.CreateSettlement;

public class CreateSettlementCommandHandler : ICommandHandler<CreateSettlementCommand, Guid>
{
    private readonly ISettlementRepository settlementRepository;
    private readonly IUnitOfWork unitOfWork;

    public CreateSettlementCommandHandler(
        ISettlementRepository settlementRepository,
        IUnitOfWork unitOfWork
    )
    {
        this.settlementRepository = settlementRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateSettlementCommand request, CancellationToken cancellationToken)
    {
        var settlementResult = Settlement.Create(request.Name);

        if (settlementResult.IsFailure)
        {
            return Result.Failure<Guid>(settlementResult.Error);
        }
        
        settlementRepository.Add(settlementResult.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return settlementResult.Value.Id.Value;
    }
}