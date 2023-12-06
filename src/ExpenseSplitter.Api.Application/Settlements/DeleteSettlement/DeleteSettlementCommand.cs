using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Settlements.CreateSettlement;

public sealed record DeleteSettlementCommand(Guid Id) : ICommand;
