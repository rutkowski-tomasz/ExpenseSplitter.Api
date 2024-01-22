using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;
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
        fixture.Customize<Expense>(x => 
            x.FromFactory(
                (string name, Amount amount, DateTime dateTime)
                    => Expense.Create(name, amount, DateOnly.FromDateTime(dateTime), SettlementId.New(), ParticipantId.New()).Value
            )
            .Without(y => y.Allocations)
        );
        fixture.Customize<User>(x => x.FromFactory(
            (string nickname, string email) =>
                User.Create(nickname, email, UserId.New()).Value
        ));
        fixture.Customize<SettlementUser>(x => x.FromFactory(
            () =>
                SettlementUser.Create(SettlementId.New(), UserId.New()).Value
        ));
        fixture.Customize<Participant>(x => x.FromFactory(
            (string nickname) =>
                Participant.Create(SettlementId.New(), nickname).Value
        ));

        return fixture;
    }
}
