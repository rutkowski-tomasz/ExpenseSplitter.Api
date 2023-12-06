using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Settlements.CreateSettlement;

public class DeleteSettlementCommandHandler : ICommandHandler<DeleteSettlementCommand>
{
    private readonly ISettlementUserRepository settlementUserRepository;
    private readonly ISettlementRepository settlementRepository;
    private readonly IUnitOfWork unitOfWork;

    public DeleteSettlementCommandHandler(
        ISettlementUserRepository settlementUserRepository,
        ISettlementRepository settlementRepository,
        IUnitOfWork unitOfWork
    )
    {
        this.settlementUserRepository = settlementUserRepository;
        this.settlementRepository = settlementRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteSettlementCommand request, CancellationToken cancellationToken)
    {
        var settlementId = new SettlementId(request.Id);
        if (!await settlementUserRepository.CanUserAccessSettlement(settlementId, cancellationToken))
        {
            return Result.Failure<Guid>(SettlementErrors.Forbidden);
        }

        var isDeleted = await settlementRepository.RemoveSettlementById(settlementId);
        
        if (!isDeleted)
        {
            return Result.Failure(SettlementErrors.NotFound);
        }

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}