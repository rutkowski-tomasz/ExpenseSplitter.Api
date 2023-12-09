using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Settlements.Events;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Domain.Settlements;

public sealed class Settlement : Entity<SettlementId>
{
    private Settlement(
        SettlementId id,
        string name,
        string inviteCode,
        UserId creatorUserId
    ) : base(id)
    {
        Name = name;
        InviteCode = inviteCode;
        CreatorUserId = creatorUserId;
    }

    public string Name { get; private set; }
    public string InviteCode { get; private set; }
    public UserId CreatorUserId { get; private set; }

    public static Result<Settlement> Create(string name, string inviteCode, UserId creatorUserId)
    {
        if (string.IsNullOrEmpty(name))
        {
            return Result.Failure<Settlement>(SettlementErrors.EmptyName);
        }
        
        var settlement = new Settlement(SettlementId.New(), name, inviteCode, creatorUserId);

        settlement.RaiseDomainEvent(new SettlementCreatedDomainEvent(settlement.Id));
        return settlement;
    }

    public void SetName(string name)
    {
        Name = name;
    }
}
