using ExpenseSplitter.Api.Application.Expenses.CreateExpense;
using ExpenseSplitter.Api.Application.Expenses.GetExpensesForSettlement;
using ExpenseSplitter.Api.Application.Settlements.GetAllSettlements;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Presentation.Expenses.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExpenseSplitter.Api.Presentation.Expenses;

public static class ExpensesEndpoints
{
    public static IEndpointRouteBuilder MapExpensesEndpoints(this IEndpointRouteBuilder builder)
    {
        var routeGroupBuilder = builder.MapGroup("api/expenses").RequireAuthorization();

        routeGroupBuilder.MapPost("", CreateExpense);
        routeGroupBuilder.MapGet("{settlementId}", GetExpensesForSettlement);

        return builder;
    }

    public static async Task<Results<Ok<Guid>, BadRequest<Error>>> CreateExpense(
        CreateExpenseRequest request,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new CreateExpenseCommand(request.Name, request.SettlementId, request.PayingParticipantId);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            TypedResults.BadRequest(result.Error);
        }

        return TypedResults.Ok(result.Value);
    }
    
    public static async Task<Results<Ok<GetExpensesForSettlementQueryResult>, BadRequest<Error>>> GetExpensesForSettlement(
        Guid settlementId,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var query = new GetExpensesForSettlementQuery(settlementId);

        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.BadRequest(result.Error);
    }
}