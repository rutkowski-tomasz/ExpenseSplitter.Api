using ExpenseSplitter.Application.Abstractions.Behaviors;
using ExpenseSplitter.Application.Settlements.CreateSettlement;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseSplitter.Application.UnitTests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddApplication_ShouldAddMediatR()
    {
        var services = new ServiceCollection();

        var result = DependencyInjection.AddApplication(services);

        services.Should().Contain(x => x.ServiceType == typeof(IPublisher));
        services.Should().Contain(x => x.ImplementationType == typeof(ValidationBehavior<,>));
        services.Should().Contain(x => x.ImplementationType == typeof(LoggingBehavior<,>));
    }

    [Fact]
    public void AddApplication_ShouldAddValidators()
    {
        var services = new ServiceCollection();

        DependencyInjection.AddApplication(services);

        services.Should().Contain(x => x.ServiceType == typeof(IValidator<CreateSettlementCommand>));
    }
}
