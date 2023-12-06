using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.SettlementUsers;

public class SettlementUserErrors
{
    public static Error AlreadyJoined = new(
        "SettlementUser.AlreadyJoined",
        "User already joined this settlement"
    );
}
