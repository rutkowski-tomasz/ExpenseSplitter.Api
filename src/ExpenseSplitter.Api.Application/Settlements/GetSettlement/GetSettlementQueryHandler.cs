using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Settlements.GetSettlement;

internal sealed class GetSettlementQueryHandler : IQueryHandler<GetSettlementQuery, GetSettlementQueryResult>
{
    private readonly ISettlementRepository settlementRepository;
    private readonly ISettlementUserRepository settlementUserRepository;
    private readonly IParticipantRepository participantRepository;
    private readonly IExpenseRepository expenseRepository;

    public GetSettlementQueryHandler(
        ISettlementRepository settlementRepository,
        ISettlementUserRepository settlementUserRepository,
        IParticipantRepository participantRepository,
        IExpenseRepository expenseRepository
    )
    {
        this.settlementRepository = settlementRepository;
        this.settlementUserRepository = settlementUserRepository;
        this.participantRepository = participantRepository;
        this.expenseRepository = expenseRepository;
    }

    public async Task<Result<GetSettlementQueryResult>> Handle(GetSettlementQuery request, CancellationToken cancellationToken)
    {
        var settlementId = new SettlementId(request.SettlementId);
        var settlementUser = await settlementUserRepository.GetBySettlementId(settlementId, cancellationToken);
        if (settlementUser is null)
        {
            return Result.Failure<GetSettlementQueryResult>(SettlementErrors.Forbidden);
        }

        var settlement = await settlementRepository.GetById(settlementId, cancellationToken);
        if (settlement is null)
        {
            return Result.Failure<GetSettlementQueryResult>(SettlementErrors.NotFound);
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

    private (decimal, decimal?) CalculateTotalAndUserCost(IEnumerable<Expense> expenses, ParticipantId? claimedParticipantId)
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