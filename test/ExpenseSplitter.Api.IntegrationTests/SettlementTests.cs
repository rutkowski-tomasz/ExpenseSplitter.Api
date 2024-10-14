using ExpenseSplitter.Api.Application.Settlements.CreateSettlement;
using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.IntegrationTests;

public class SettlementTests(IntegrationTestWebAppFactory appFactory) : BaseIntegrationTest(appFactory)
{
    [Fact]
    public async Task Create_ShouldAddNewSettlementToDatabase()
    {
        var command = new Fixture().Create<CreateSettlementCommand>();

        var result = await Sender.Send(command);

        result.Value.Should().NotBeEmpty();

        DbContext
            .Set<Settlement>()
            .FirstOrDefault(x => x.Id == new SettlementId(result.Value))
            .Should()
            .NotBeNull();
    }
}
