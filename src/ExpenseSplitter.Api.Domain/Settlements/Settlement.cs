using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Settlements.Events;

namespace ExpenseSplitter.Api.Domain.Settlements;

public sealed class Settlement : Entity<SettlementId>
{
    private Settlement(
        SettlementId id,
        string name,
        string inviteCode
    ) : base(id)
    {
        Name = name;
        InviteCode = inviteCode;
    }

    public string Name { get; private set; }
    public string InviteCode { get; private set; }

    public static Result<Settlement> Create(string name, string inviteCode)
    {
        if (string.IsNullOrEmpty(name))
        {
            return Result.Failure<Settlement>(SettlementErrors.EmptyName);
        }
        
        var settlement = new Settlement(SettlementId.New(), name, inviteCode);

        settlement.RaiseDomainEvent(new SettlementCreatedDomainEvent(settlement.Id));
        return settlement;
    }
}
