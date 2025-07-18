using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Application.Abstractions.Etag;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Application.Settlements.DeleteSettlement;

public class DeleteSettlementCommandHandler(
    ISettlementRepository settlementRepository,
    IUserContext userContext,
    IUnitOfWork unitOfWork,
    IEtagService etagService
) : ICommandHandler<DeleteSettlementCommand>
{
    public async Task<Result> Handle(DeleteSettlementCommand request, CancellationToken cancellationToken)
    {
        var settlementId = new SettlementId(request.Id);
        var settlement = await settlementRepository.GetById(settlementId, cancellationToken);

        if (settlement is null)
        {
            return Result.Failure(SettlementErrors.NotFound);
        }
        
        if (etagService.HasIfMatchConflict(settlement))
        {
            return Result.Failure(SettlementErrors.IfMatchHeaderConflict);
        }

        if (settlement.CreatorUserId != userContext.UserId)
        {
            return Result.Failure(SettlementErrors.Forbidden);
        }

        settlementRepository.Remove(settlement);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
