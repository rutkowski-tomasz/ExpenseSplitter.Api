using ExpenseSplitter.Api.Domain.Abstractions;
using NetArchTest.Rules;

namespace ExpenseSplitter.Api.ArchitectureTests;

public class DomainTests
{
    [Fact]
    public void Test()
    {
        var result = Types
            .InAssembly(Assemblies.Domain)
            .That()
            .ImplementInterface(typeof(DomainEvent))
            .Should()
            .HaveNameEndingWith("DomainEvent")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
