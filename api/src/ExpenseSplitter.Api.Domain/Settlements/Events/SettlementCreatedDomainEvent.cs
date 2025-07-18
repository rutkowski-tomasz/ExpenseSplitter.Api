using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Settlements.Events;

public record SettlementCreatedDomainEvent(SettlementId Id) : IDomainEvent;
