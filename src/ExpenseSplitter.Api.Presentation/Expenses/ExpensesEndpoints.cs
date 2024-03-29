using ExpenseSplitter.Api.Application.Expenses.CreateExpense;
using ExpenseSplitter.Api.Application.Expenses.DeleteExpense;
using ExpenseSplitter.Api.Application.Expenses.GetExpense;
using ExpenseSplitter.Api.Application.Expenses.UpdateExpense;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Expenses.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExpenseSplitter.Api.Presentation.Expenses;

public class ExpensesEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var routeGroupBuilder = builder.MapGroup("expenses").RequireAuthorization();

        routeGroupBuilder.MapPost("", CreateExpense);
        routeGroupBuilder.MapGet("{expenseId}", GetExpense);
        routeGroupBuilder.MapPut("{expenseId}", UpdateExpense);
        routeGroupBuilder.MapDelete("{expenseId}", DeleteExpense);
    }

    public static async Task<Results<Ok<Guid>, BadRequest<Error>>> CreateExpense(
        CreateExpenseRequest request,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new CreateExpenseCommand(
            request.Name,
            request.PaymentDate,
            request.SettlementId,
            request.PayingParticipantId,
            request.Allocations.Select(x => new CreateExpenseCommandAllocation(
                x.ParticipantId,
                x.Value
            ))
        );

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error);
        }

        return TypedResults.Ok(result.Value);
    }
    
    public static async Task<Results<Ok<GetExpenseQueryResult>, BadRequest<Error>>> GetExpense(
        Guid expenseId,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var query = new GetExpenseQuery(expenseId);

        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.BadRequest(result.Error);
    }

    public static async Task<Results<Ok, BadRequest<Error>>> UpdateExpense(
        UpdateExpenseRequest request,
        Guid expenseId,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new UpdateExpenseCommand(
            expenseId,
            request.Title,
            request.PaymentDate,
            request.PayingParticipantId,
            request.Allocations.Select(x => new UpdateExpenseCommandAllocation(
                x.Id,
                x.ParticipantId,
                x.Value
            ))
        );

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok() : TypedResults.BadRequest(result.Error);
    }

    public static async Task<Results<Ok, BadRequest<Error>>> DeleteExpense(
        Guid expenseId,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var query = new DeleteExpenseCommand(expenseId);

        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok() : TypedResults.BadRequest(result.Error);
    }
}
