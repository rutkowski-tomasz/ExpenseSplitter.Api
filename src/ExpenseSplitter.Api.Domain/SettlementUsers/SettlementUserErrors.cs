using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.SettlementUsers;

public static class SettlementUserErrors
{
    public static readonly AppError AlreadyJoined = new(
        ErrorType.BadRequest,
        "User already joined this settlement"
    );
}
