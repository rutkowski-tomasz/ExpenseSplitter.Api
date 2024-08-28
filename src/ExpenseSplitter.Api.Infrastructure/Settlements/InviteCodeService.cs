using System.Security.Cryptography;
using ExpenseSplitter.Api.Application.Settlements.CreateSettlement;

namespace ExpenseSplitter.Api.Infrastructure.Settlements;

public class InviteCodeService : IInviteCodeService
{
    private const string InviteCodeChars = "abcdefghjkmnpqrstwxyzABCDEFGHJKLMNOPQRSTWXYZ23456789";
    private const int InviteCodeLength = 8;
    
    public string GenerateInviteCode()
    {
        

        var inviteCode = string.Join(
            "",
            Enumerable
                .Range(1, InviteCodeLength)
                .Select(_ => RandomNumberGenerator.GetInt32(InviteCodeChars.Length))
                .Select(x => InviteCodeChars[x])
        );

        return inviteCode;
    }
}
