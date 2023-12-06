using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Settlements.CreateSettlement;

public class DeleteSettlementCommandHandler : ICommandHandler<DeleteSettlementCommand>
{
    private readonly ISettlementRepository settlementRepository;
    private readonly IUserContext userContext;
    private readonly IUnitOfWork unitOfWork;

    public DeleteSettlementCommandHandler(
        ISettlementRepository settlementRepository,
        IUserContext userContext,
        IUnitOfWork unitOfWork
    )
    {
        this.settlementRepository = settlementRepository;
        this.userContext = userContext;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteSettlementCommand request, CancellationToken cancellationToken)
    {
        var settlementId = new SettlementId(request.Id);
        var settlement = await settlementRepository.GetByIdAsync(settlementId, cancellationToken);
        if (settlement is null)
        {
            return Result.Failure<Guid>(SettlementErrors.NotFound);
        }

        if (settlement.CreatorUserId != userContext.UserId)
        {
            return Result.Failure<Guid>(SettlementErrors.Forbidden);
        }

        settlementRepository.Remove(settlement);
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}