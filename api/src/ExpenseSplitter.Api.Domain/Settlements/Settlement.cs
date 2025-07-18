using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Common;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements.Events;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Domain.Settlements;

public sealed class Settlement : AggregateRoot<SettlementId>
{
    private readonly List<Participant> participants = [];

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
    public IReadOnlyList<Participant> Participants => participants.AsReadOnly();

    public static Result<Settlement> Create(string name, string inviteCode, UserId creatorUserId, DateTime createdOnUtc)
    {
        if (string.IsNullOrEmpty(name))
        {
            return Result.Failure<Settlement>(SettlementErrors.EmptyName);
        }
        
        var settlement = new Settlement(SettlementId.New(), name, inviteCode, creatorUserId, createdOnUtc);

        settlement.AddOnSaveDomainEvent(new SettlementCreatedDomainEvent(settlement.Id));
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

    public Result AddParticipant(string nickname)
    {
        var participantResult = Participant.Create(Id, nickname);
        if (participantResult.IsFailure)
        {
            return participantResult;
        }
        
        participants.Add(participantResult.Value);
        return Result.Success();
    }
    
    public Result UpdateParticipant(ParticipantId participantId, string nickname)
    {
        var participant = participants.FirstOrDefault(x => x.Id == participantId);
        if (participant is null)
        {
            return SettlementErrors.ParticipantNotFound;
        }

        var setNicknameResult = participant.SetNickname(nickname);
        return setNicknameResult;
    }
    
    public Result RemoveParticipant(ParticipantId participantId)
    {
        var allocation = participants.FirstOrDefault(x => x.Id == participantId);
        if (allocation is null)
        {
            return Result.Failure(SettlementErrors.ParticipantNotFound);
        }

        participants.Remove(allocation);

        return Result.Success();
    }

    public bool IsParticipantInSettlement(ParticipantId participantId)
    {
        return participants.Select(x => x.Id).Contains(participantId);
    }

    public bool AreAllParticipantsInSettlement(IEnumerable<ParticipantId> participantIds)
    {
        return participantIds.All(participants.Select(x => x.Id).Contains);
    }
}
