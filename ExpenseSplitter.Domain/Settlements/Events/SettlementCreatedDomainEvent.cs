using ExpenseSplitter.Domain.Abstractions;

namespace ExpenseSplitter.Domain.Settlements.Events;

public record SettlementCreatedDomainEvent(SettlementId Id) : IDomainEvent;
