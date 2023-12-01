using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Infrastructure;
using System.Reflection;
using ExpenseSplitter.Api.Presentation.Settlements;

namespace ExpenseSplitter.Api.ArchitectureTests;

public class Assemblies
{
    public static Assembly Domain => typeof(Entity<>).Assembly;

    public static Assembly Application => typeof(IBaseCommand).Assembly;

    public static Assembly Infrastructure => typeof(ApplicationDbContext).Assembly;

    public static Assembly Presentation => typeof(SettlementEndpoints).Assembly;
}
