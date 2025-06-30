using System.Diagnostics.Metrics;
using ExpenseSplitter.Api.Application.Abstractions.Metrics;

namespace ExpenseSplitter.Api.Infrastructure.Metrics;

public class MetricsService : IMetricsService
{
    public const string ServiceName = "ExpenseSplitter.Api";

    private readonly Counter<long> registeredUsersCounter;
    private readonly Counter<long> activeUsersCounter;

    public MetricsService(IMeterFactory meterFactory)
    {
        using var meter = meterFactory.Create(ServiceName);
        registeredUsersCounter = meter.CreateCounter<long>($"{ServiceName}.users.registered.total", "Number of registered users");
        activeUsersCounter = meter.CreateCounter<long>($"{ServiceName}.users.active.total", "Number of active users");
    }

    public void RecordRegisteredUser()
    {
        registeredUsersCounter.Add(1);
    }

    public void RecordActiveUser()
    {
        activeUsersCounter.Add(1);
    }
} 
