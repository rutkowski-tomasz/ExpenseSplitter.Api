using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Application.Abstractions.Idempotency;

namespace ExpenseSplitter.Api.Application.Settlements.CreateSettlement;

public sealed record CreateSettlementCommand(
    Guid RequestId,
    string Name,
    IEnumerable<string> ParticipantNames
) : ICommand<Guid>, IIdempotentCommand;
