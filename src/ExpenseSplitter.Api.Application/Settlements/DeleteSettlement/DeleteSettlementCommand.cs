using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Settlements.DeleteSettlement;

public sealed record DeleteSettlementCommand(Guid Id) : ICommand;
