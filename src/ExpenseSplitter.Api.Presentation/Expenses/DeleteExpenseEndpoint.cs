using ExpenseSplitter.Api.Application.Expenses.DeleteExpense;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;

namespace ExpenseSplitter.Api.Presentation.Expenses;

public class DeleteExpenseEndpoint : IEndpoint,
    IMapper<Guid, DeleteExpenseCommand>
{
    public DeleteExpenseCommand Map(Guid source)
    {
        return new DeleteExpenseCommand(source);
    }

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Expenses()
            .MapDelete("{expenseId}", (
                Guid expenseId,
                IHandlerEmptyResponse<
                    Guid,
                    DeleteExpenseCommand
                > handler) => handler.Handle(expenseId)
            )
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound);
    }
}
