using ExpenseSplitter.Api.Application.Expenses.DeleteExpense;
using ExpenseSplitter.Api.Presentation.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Presentation.Expenses;

public record DeleteExpenseRequest(
    [FromRoute] Guid ExpenseId
);

public class DeleteExpenseEndpoint() : Endpoint<DeleteExpenseRequest, DeleteExpenseCommand>(
    Route: "{expenseId}",
    Group: EndpointGroup.Expenses,
    Method: EndpointMethod.Delete,
    MapRequest: request => new (request.ExpenseId),
    ErrorStatusCodes: [
        StatusCodes.Status403Forbidden,
        StatusCodes.Status404NotFound
    ]
);
