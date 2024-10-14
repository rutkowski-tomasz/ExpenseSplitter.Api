using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Application.Abstractions.Etag;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Settlements.GetSettlement;

internal sealed class GetSettlementQueryHandler(
    ISettlementRepository settlementRepository,
    ISettlementUserRepository settlementUserRepository,
    IParticipantRepository participantRepository,
    IExpenseRepository expenseRepository,
    IEtagService etagService
) : IQueryHandler<GetSettlementQuery, GetSettlementQueryResult>
{
    public async Task<Result<GetSettlementQueryResult>> Handle(GetSettlementQuery request, CancellationToken cancellationToken)
    {
        var settlementId = new SettlementId(request.SettlementId);
        var settlementUser = await settlementUserRepository.GetBySettlementId(settlementId, cancellationToken);
        if (settlementUser is null)
        {
            return SettlementErrors.Forbidden;
        }

        var settlement = await settlementRepository.GetById(settlementId, cancellationToken);
        if (settlement is null)
        {
            return SettlementErrors.NotFound;
        }

        etagService.AttachEtagToResponse(settlement);
        if (etagService.HasIfNoneMatchConflict(settlement))
        {
            return SettlementErrors.IfNoneMatchNotModified;
        }

        var participants = await participantRepository.GetAllBySettlementId(settlementId, cancellationToken);

        var expenses = await expenseRepository.GetAllBySettlementId(settlementId, cancellationToken);
        var (totalCost, yourCost) = CalculateTotalAndUserCost(expenses, settlementUser.ParticipantId);

        var settlementDto = new GetSettlementQueryResult(
            settlement.Id.Value,
            settlement.Name,
            settlement.InviteCode,
            totalCost,
            yourCost,
            participants.Select(x => new GetSettlementQueryResultParticipant(
                x.Id.Value,
                x.Nickname
            ))
        );

        return settlementDto;
    }

    private static (decimal, decimal?) CalculateTotalAndUserCost(IEnumerable<Expense> expenses, ParticipantId? claimedParticipantId)
    {
        var total = 0m;
        var userCost = 0m;
        
        foreach (var expense in expenses)
        {
            total += expense.Amount.Value;

            var userAllocation = expense
                .Allocations
                .FirstOrDefault(x => x.ParticipantId == claimedParticipantId);

            if (userAllocation is not null)
            {
                userCost += userAllocation.Amount.Value;
            }
        }

        return claimedParticipantId is null
            ? (total, null)
            : (total, userCost);
    }
}
