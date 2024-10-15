namespace ExpenseSplitter.Api.Application.Settlements.CalculateReimbursement;

public sealed record CalculateReimbursementQueryResult(
    IEnumerable<CalculateReimbursementQueryResultBalance> Balances,
    IEnumerable<CalculateReimbursementQueryResultSuggestedReimbursement> SuggestedReimbursements
);

public sealed record CalculateReimbursementQueryResultBalance(
    Guid ParticipantId,
    decimal Value
);

public sealed record CalculateReimbursementQueryResultSuggestedReimbursement(
    Guid FromParticipantId,
    Guid ToParticipantId,
    decimal Value
);
