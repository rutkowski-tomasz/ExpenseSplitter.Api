using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Settlements.Events;
using Microsoft.Extensions.Logging;

namespace ExpenseSplitter.Api.Application.Settlements.CreateSettlement;

public class SettlementCreatedDomainEventHandler : IDomainEventHandler<SettlementCreatedDomainEvent>
{
    private readonly ILogger<SettlementCreatedDomainEventHandler> logger;

    public SettlementCreatedDomainEventHandler(
        ILogger<SettlementCreatedDomainEventHandler> logger
    )
    {
        this.logger = logger;
    }

    public Task Handle(SettlementCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Created settlement {Id}", notification.Id.Value);
        return Task.CompletedTask;
    }
}
