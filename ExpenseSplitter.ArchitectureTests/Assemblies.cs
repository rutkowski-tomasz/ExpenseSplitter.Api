using ExpenseSplitter.Application.Abstractions.Cqrs;
using ExpenseSplitter.Domain.Abstractions;
using ExpenseSplitter.Infrastructure;
using ExpenseSplitter.Presentation.Api.Endpoints;
using System.Reflection;

namespace ExpenseSplitter.ArchitectureTests;

public class Assemblies
{
    public static Assembly Domain => typeof(Entity<>).Assembly;

    public static Assembly Application => typeof(IBaseCommand).Assembly;

    public static Assembly Infrastructure => typeof(ApplicationDbContext).Assembly;

    public static Assembly Presentation => typeof(SettlementEndpoints).Assembly;
}
