namespace ExpenseSplitter.Api.Presentation.Settlements.Models;

public sealed record UpdateSettlementRequest(
    string Name,
    IEnumerable<UpdateSettlementRequestParticipant> Participants
);

public sealed record UpdateSettlementRequestParticipant(
    Guid? Id,
    string Nickname
);