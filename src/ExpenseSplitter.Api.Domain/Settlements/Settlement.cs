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
        UserId creatorUserId,
        DateTime createdOnUtc
    ) : base(id)
    {
        Name = name;
        InviteCode = inviteCode;
        CreatorUserId = creatorUserId;
        CreatedOnUtc = createdOnUtc;
        UpdatedOnUtc = createdOnUtc;
    }

    public string Name { get; private set; }
    public string InviteCode { get; private set; }
    public UserId CreatorUserId { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime UpdatedOnUtc { get; private set; }

    public static Result<Settlement> Create(string name, string inviteCode, UserId creatorUserId, DateTime createdOnUtc)
    {
        if (string.IsNullOrEmpty(name))
        {
            return Result.Failure<Settlement>(SettlementErrors.EmptyName);
        }
        
        var settlement = new Settlement(SettlementId.New(), name, inviteCode, creatorUserId, createdOnUtc);

        settlement.AddPersistDomainEvent(new SettlementCreatedDomainEvent(settlement.Id));
        return settlement;
    }

    public Result SetName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return Result.Failure(SettlementErrors.EmptyName);
        }

        Name = name;
        return Result.Success();
    }

    public void SetUpdatedOnUtc(DateTime updatedOnUtc)
    {
        UpdatedOnUtc = updatedOnUtc;
    }
}
