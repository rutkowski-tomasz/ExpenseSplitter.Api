using ExpenseSplitter.Api.Application.Expenses.DeleteExpense;
using ExpenseSplitter.Api.Presentation.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Presentation.Expenses;

public record DeleteExpenseRequest(
    [FromRoute] Guid ExpenseId
);

public class DeleteExpenseEndpoint() : Endpoint<DeleteExpenseRequest, DeleteExpenseCommand>(
    Endpoints.Expenses.Delete("{expenseId}").ProducesErrorCodes(
        StatusCodes.Status403Forbidden,
        StatusCodes.Status404NotFound
    ),
    request => new (request.ExpenseId)
);
