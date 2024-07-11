using ExpenseSplitter.Api.Application.Expenses.DeleteExpense;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
using MediatR;

namespace ExpenseSplitter.Api.Presentation.Expenses;

public class DeleteExpenseEndpoint : EndpointEmptyResponse<Guid, DeleteExpenseCommand>
{
    public override DeleteExpenseCommand MapRequest(Guid source)
    {
        return new DeleteExpenseCommand(source);
    }

    public override void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Expenses()
            .MapDelete("{expenseId}", (Guid expenseId, ISender sender)
                => Handle(expenseId, sender))
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound);
    }
}
