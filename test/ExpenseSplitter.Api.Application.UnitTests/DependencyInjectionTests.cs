using ExpenseSplitter.Api.Application.Abstractions.Behaviors;
using ExpenseSplitter.Api.Application.Settlements.CreateSettlement;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseSplitter.Api.Application.UnitTests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddApplication_ShouldAddMediatR()
    {
        var services = new ServiceCollection();

        services.AddApplication();

        services.Should().Contain(x => x.ServiceType == typeof(IPublisher));
        services.Should().Contain(x => x.ImplementationType == typeof(ValidationBehavior<,>));
        services.Should().Contain(x => x.ImplementationType == typeof(LoggingBehavior<,>));
    }

    [Fact]
    public void AddApplication_ShouldAddValidators()
    {
        var services = new ServiceCollection();

        services.AddApplication();

        services.Should().Contain(x => x.ServiceType == typeof(IValidator<CreateSettlementCommand>));
    }
}
