using ExpenseSplitter.Api.Application.Settlements.CreateSettlement;

namespace ExpenseSplitter.Api.Infrastructure.Settlements;

public class InviteCodeService : IInviteCodeService
{
    private const string InviteCodeChars = "abcdefghjkmnpqrstwxyzABCDEFGHJKLMNOPQRSTWXYZ23456789";
    private const int InviteCodeLength = 8;
    
    public string GenerateInviteCode()
    {
        var random = new Random();

        var inviteCode = string.Join(
            "",
            Enumerable
                .Range(1, InviteCodeLength)
                .Select(_ => InviteCodeChars[random.Next(InviteCodeChars.Length)])
        );

        return inviteCode;
    }
}