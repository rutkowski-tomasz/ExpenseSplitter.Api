using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Settlements.CalculateReimbrusement;

internal sealed class CalculateReimbrusementQueryHandler(
    ISettlementUserRepository settlementUserRepository,
    IExpenseRepository expenseRepository,
    IParticipantRepository participantRepository
) : IQueryHandler<CalculateReimbrusementQuery, CalculateReimbrusementQueryResult>
{
    public async Task<Result<CalculateReimbrusementQueryResult>> Handle(CalculateReimbrusementQuery request, CancellationToken cancellationToken)
    {
        var settlementId = new SettlementId(request.SettlementId);
        if (!await settlementUserRepository.CanUserAccessSettlement(settlementId, cancellationToken))
        {
            return SettlementErrors.Forbidden;
        }

        var expenses = await expenseRepository.GetAllBySettlementId(settlementId, cancellationToken);
        var participants = await participantRepository.GetAllBySettlementId(settlementId, cancellationToken);

        var balances = CalculateBalances(expenses, participants);
        var suggestedReimbursements = CalculateSuggestedReimbrusements(balances);

        return new CalculateReimbrusementQueryResult(
            balances,
            suggestedReimbursements
        );
    }

    private IEnumerable<CalculateReimbrusementQueryResultBalance> CalculateBalances(
        IEnumerable<Expense> expenses,
        IEnumerable<Participant> participants
    )
    {
        var balances = participants.ToDictionary(x => x.Id, _ => 0m);
        foreach (var expense in expenses)
        {
            balances[expense.PayingParticipantId] += expense.Amount.Value;

            foreach (var allocation in expense.Allocations)
            {
                balances[allocation.ParticipantId] -= allocation.Amount.Value;
            }
        }

        return balances.Select(x => new CalculateReimbrusementQueryResultBalance(x.Key.Value, x.Value));
    }

    private IEnumerable<CalculateReimbrusementQueryResultSuggestedReimbrusement> CalculateSuggestedReimbrusements(
        IEnumerable<CalculateReimbrusementQueryResultBalance> balances
    )
    {
        var suggestedReimbursements = new List<CalculateReimbrusementQueryResultSuggestedReimbrusement>();

        var localBalances = balances.ToDictionary(b => b.ParticipantId, b => b.Value);
        var debtorIds = balances.Where(b => b.Value < 0).Select(x => x.ParticipantId).ToList();
        var creditorIds = balances.Where(b => b.Value > 0).Select(x => x.ParticipantId).ToList();

        foreach (var debtorId in debtorIds)
        {
            while (localBalances[debtorId] < 0)
            {
                var creditorId = localBalances.First(x => x.Value > 0).Key;
                var amount = Math.Min(-localBalances[debtorId], localBalances[creditorId]);

                suggestedReimbursements.Add(new CalculateReimbrusementQueryResultSuggestedReimbrusement(
                    debtorId,
                    creditorId,
                    amount
                ));

                localBalances[debtorId] += amount;
                localBalances[creditorId] -= amount;
            }
        }

        return suggestedReimbursements;
    }
}
