using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.SettlementUsers;

public static class SettlementUserErrors
{
    public static readonly Error AlreadyJoined = new(
        "SettlementUser.AlreadyJoined",
        "User already joined this settlement"
    );
}
