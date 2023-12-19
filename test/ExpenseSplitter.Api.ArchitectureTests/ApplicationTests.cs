using System.Reflection.Metadata;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using FluentValidation;
using MediatR;
using NetArchTest.Rules;

namespace ExpenseSplitter.Api.ArchitectureTests;

public class ApplicationTests
{
    [Fact]
    public void Queries_ShouldHaveQueryEnding()
    {
        var result = Types
          .InAssembly(Assemblies.Application)
          .That()
          .ImplementInterface(typeof(IQuery<>))
          .Should()
          .HaveNameEndingWith("Query")
          .And()
          .BeSealed()
          .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void QueriesResults_ShouldReturnTypesEndingWithQueryResult()
    {
        var result = Types
            .InAssembly(Assemblies.Application)
            .That()
            .ImplementInterface(typeof(IQuery<>))
            .Should()
            .MeetCustomRule(new GenericArgumentsEndWithCustomRule("QueryResult"))
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Commands_ShouldHaveCommandEnding()
    {
        var result = Types
          .InAssembly(Assemblies.Application)
          .That()
          .ImplementInterface(typeof(ICommand<>))
          .Should()
          .HaveNameEndingWith("Command")
          .And()
          .BeSealed()
          .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void QueryHandlers_ShouldHaveQueryHandlerEnding()
    {
        var result = Types
          .InAssembly(Assemblies.Application)
          .That()
          .ImplementInterface(typeof(IQueryHandler<,>))
          .Should()
          .HaveNameEndingWith("QueryHandler")
          .And()
          .BeSealed()
          .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void CommandHandlers_ShouldHaveCommandHandlerEnding()
    {
        var result = Types
          .InAssembly(Assemblies.Application)
          .That()
          .ImplementInterface(typeof(ICommandHandler<>))
          .Or()
          .ImplementInterface(typeof(ICommandHandler<,>))
          .Should()
          .HaveNameEndingWith("CommandHandler")
          .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Validators_ShouldHaveValidatorEnding()
    {
        var result = Types
          .InAssembly(Assemblies.Application)
          .That()
          .Inherit(typeof(AbstractValidator<>))
          .Should()
          .HaveNameEndingWith("Validator")
          .And()
          .BeSealed()
          .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Behaviors_ShouldHaveBehaviorEnding()
    {
        var result = Types
            .InAssembly(Assemblies.Application)
            .That()
            .ImplementInterface(typeof(IPipelineBehavior<,>))
            .Should()
            .HaveNameMatching("Behavior")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DomainEventHandlers_ShouldHaveDomainEventHandlerEnding()
    {
        var result = Types
            .InAssembly(Assemblies.Application)
            .That()
            .ImplementInterface(typeof(IDomainEventHandler<>))
            .Should()
            .HaveNameEndingWith("DomainEventHandler")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}

public class GenericArgumentsEndWithCustomRule : ICustomRule
{
    private readonly string endWith;

    public GenericArgumentsEndWithCustomRule(string endWith)
    {
        this.endWith = endWith;
    }

    public bool MeetsRule(Mono.Cecil.TypeDefinition type)
    {
        var interfaceType = type.Interfaces.First().InterfaceType;
        var genericInstaceType = interfaceType as Mono.Cecil.GenericInstanceType;
        var genericArguments = genericInstaceType!.GenericArguments;
        
        return genericArguments.First().Name.EndsWith(endWith);
    }
}