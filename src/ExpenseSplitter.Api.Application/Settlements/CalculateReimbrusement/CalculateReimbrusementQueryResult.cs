namespace ExpenseSplitter.Api.Application.Settlements.CalculateReimbrusement;

public sealed record CalculateReimbrusementQueryResult(
    IEnumerable<CalculateReimbrusementQueryResultBalance> Balances,
    IEnumerable<CalculateReimbrusementQueryResultSuggestedReimbrusement> SuggestedReimbrusements
);

public sealed record CalculateReimbrusementQueryResultBalance(
    Guid ParticipantId,
    decimal Value
);

public sealed record CalculateReimbrusementQueryResultSuggestedReimbrusement(
    Guid FromParticipantId,
    Guid ToParticipantId,
    decimal Value
);
