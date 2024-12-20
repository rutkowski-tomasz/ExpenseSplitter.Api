namespace ExpenseSplitter.Api.Domain.Participants;

public record ParticipantId(Guid Value)
{
    public static ParticipantId New() => new(Guid.CreateVersion7());
}
