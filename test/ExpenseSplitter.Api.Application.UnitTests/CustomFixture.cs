using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Application.UnitTests;

public abstract class CustomFixture
{
    public static Fixture Create()
    {
        var fixture = new Fixture();

        fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));
        fixture.Customize<UserId>(x => x.FromFactory(UserId.New));
        fixture.Customize<SettlementId>(x => x.FromFactory(SettlementId.New));
        fixture.Customize<ExpenseId>(x => x.FromFactory(ExpenseId.New));

        fixture.Customize<Settlement>(x => x.FromFactory(() => Settlement.Create(
            fixture.Create<string>(),
            fixture.Create<string>(),
            fixture.Create<UserId>(),
            fixture.Create<DateTime>()
        ).Value));
        
        fixture.Customize<Expense>(x => 
            x.FromFactory((
                string title,
                DateTime dateTime,
                SettlementId settlementId,
                List<ParticipantId> participantIds
            ) => Expense.Create(
                    title,
                    DateOnly.FromDateTime(dateTime),
                    settlementId,
                    participantIds[0],
                    participantIds.ToDictionary(y => y, _ => fixture.Create<decimal>())
                ).Value
            )
        );
        fixture.Customize<User>(x => x.FromFactory(
            (string nickname, string email) =>
                User.Create(nickname, email, UserId.New()).Value
        ));
        fixture.Customize<SettlementUser>(x => x.FromFactory(
            () =>
                SettlementUser.Create(SettlementId.New(), UserId.New())
        ));

        return fixture;
    }
}
