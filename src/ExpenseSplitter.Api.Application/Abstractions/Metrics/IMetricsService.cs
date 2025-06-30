namespace ExpenseSplitter.Api.Application.Abstractions.Metrics;

public interface IMetricsService
{
    void RecordRegisteredUser();
    void RecordActiveUser();
} 
