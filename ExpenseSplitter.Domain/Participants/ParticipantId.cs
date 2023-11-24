namespace ExpenseSplitter.Domain.Participants;

public record ParticipantId(Guid Value)
{
    public static ParticipantId New() => new(Guid.NewGuid());
}
