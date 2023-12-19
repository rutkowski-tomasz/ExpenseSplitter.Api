using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.Shared;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Application.UnitTests;

public class CustomFixutre
{
    public static Fixture Create()
    {
        var fixture = new Fixture();
        fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));
        fixture.Customize<Settlement>(x => x.FromFactory(
            (string name, string inviteCode, UserId creatorUserId, DateTime createdOnUtc) =>
                Settlement.Create(name, inviteCode, creatorUserId, createdOnUtc).Value
        ));
        fixture.Customize<Expense>(x => x.FromFactory(
            (string name, Amount amount, DateTime dateTime)
                => Expense.Create(name, amount, DateOnly.FromDateTime(dateTime), SettlementId.New(), ParticipantId.New()).Value
        ));

        return fixture;
    }
}
