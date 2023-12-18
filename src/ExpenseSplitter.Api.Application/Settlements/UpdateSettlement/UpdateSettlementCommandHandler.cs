using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Settlements.UpdateSettlement;

public class UpdateSettlementCommandHandler : ICommandHandler<UpdateSettlementCommand>
{
    private readonly ISettlementUserRepository settlementUserRepository;
    private readonly ISettlementRepository settlementRepository;
    private readonly IUnitOfWork unitOfWork;

    public UpdateSettlementCommandHandler(
        ISettlementUserRepository settlementUserRepository,
        ISettlementRepository settlementRepository,
        IUnitOfWork unitOfWork
    )
    {
        this.settlementUserRepository = settlementUserRepository;
        this.settlementRepository = settlementRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateSettlementCommand request, CancellationToken cancellationToken)
    {
        var settlementId = new SettlementId(request.Id);
        if (!await settlementUserRepository.CanUserAccessSettlement(settlementId, cancellationToken))
        {
            return Result.Failure(SettlementErrors.Forbidden);
        }

        var settlement = await settlementRepository.GetById(settlementId, cancellationToken);
        if (settlement is null)
        {
            return Result.Failure(SettlementErrors.NotFound);
        }

        settlement.SetName(request.Name);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
