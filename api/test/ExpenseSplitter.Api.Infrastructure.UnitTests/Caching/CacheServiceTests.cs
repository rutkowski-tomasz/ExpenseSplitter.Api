using AutoFixture;
using ExpenseSplitter.Api.Infrastructure.Caching;
using ExpenseSplitter.Api.Infrastructure.Serializer;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;

namespace ExpenseSplitter.Api.Infrastructure.UnitTests.Caching;

public class CacheServiceTests
{
    private readonly IDistributedCache distributedCache = Substitute.For<IDistributedCache>();
    private readonly ISerializer serializer = Substitute.For<ISerializer>();
    private readonly Fixture fixture = new();
    private readonly DistributedCacheService cacheService;

    public CacheServiceTests()
    {
        cacheService = new DistributedCacheService(distributedCache, serializer);
    }

    [Fact]
    public async Task GetOrCreateAsync_ShouldUseCachedValue()
    {
        var key = fixture.Create<string>();
        var value = fixture.Create<string>();
        var cachedBytes = fixture.CreateMany<byte>().ToArray();
        var factoryValue = fixture.Create<string>();

        distributedCache
            .GetAsync(key, Arg.Any<CancellationToken>())
            .Returns(cachedBytes);

        serializer
            .Deserialize<string>(cachedBytes, Arg.Any<CancellationToken>())
            .Returns(value);

        var result = await cacheService.GetOrCreate(
            key,
            _ => Task.FromResult(factoryValue)
        );

        result.Should().Be(value);
    }

    [Fact]
    public async Task GetOrCreateAsync_ShouldUseFactoryFunction_WhenKeyIsNotSet()
    {
        var key = fixture.Create<string>();

        var factoryValue = fixture.Create<string>();

        var result = await cacheService.GetOrCreate(
            key,
            _ => Task.FromResult(factoryValue)
        );

        result.Should().Be(factoryValue);
    }

    [Fact]
    public async Task GetOrCreateAsync_ShouldNotCallFactory_GivenNullInCache()
    {
        var key = fixture.Create<string>();
        var cachedNull = new byte[] { 192 };
        var factoryValue = fixture.Create<string>();

        distributedCache
            .GetAsync(key, Arg.Any<CancellationToken>())
            .Returns(cachedNull);

        serializer
            .Deserialize<string>(cachedNull, Arg.Any<CancellationToken>())
            .Returns((string?) null);

        var result = await cacheService.GetOrCreate(
            key,
            _ => Task.FromResult(factoryValue)
        );

        result.Should().BeNull();
    }
    
    [Fact]
    public async Task BatchGetOrCreateAsync_ShouldReturnAllFromCache_WhenAllKeysAreCached()
    {
        var keys = new List<string> { "Key1", "Key2", "Key3" };
        var cachedValues = new Dictionary<string, string>
        {
            { "Key1", "Value1" },
            { "Key2", "Value2" },
            { "Key3", "Value3" }
        };

        foreach (var key in keys)
        {
            var cacheKey = key;
            var cachedBytes = fixture.CreateMany<byte>().ToArray();
            distributedCache.GetAsync(cacheKey, Arg.Any<CancellationToken>()).Returns(cachedBytes);
            serializer.Deserialize<string>(cachedBytes, Arg.Any<CancellationToken>()).Returns(cachedValues[key]);
        }

        var result = await cacheService.BatchGetOrCreate(
            keys,
            (_, _) => Task.FromResult(new Dictionary<string, string>()),
            null,
            CancellationToken.None
        );
        
        result.Should().BeEquivalentTo(cachedValues);
    }

    [Fact]
    public async Task BatchGetOrCreateAsync_ShouldUseFactoryForMissingKeys_WhenSomeKeysAreNotCached()
    {
        var keys = new List<string> { "Key1", "Key2", "Key3" };
        var cachedValues = new Dictionary<string, string>
        {
            { "Key1", "Value1" },
            { "Key3", "Value3" }
        };
        var factoryValues = new Dictionary<string, string>
        {
            { "Key2", "FactoryValue2" }
        };

        foreach (var key in keys)
        {
            var cacheKey = key;
            if (cachedValues.TryGetValue(key, out var value))
            {
                var cachedBytes = fixture.CreateMany<byte>().ToArray();
                distributedCache.GetAsync(cacheKey, Arg.Any<CancellationToken>()).Returns(cachedBytes);
                serializer.Deserialize<string>(cachedBytes, Arg.Any<CancellationToken>()).Returns(value);
            }
            else
            {
                distributedCache.GetAsync(cacheKey, Arg.Any<CancellationToken>()).Returns((byte[]?)null);
            }
        }

        var result = await cacheService.BatchGetOrCreate(
            keys,
            (_, _) => Task.FromResult(factoryValues),
            null,
            CancellationToken.None
        );

        var expectedResult = new Dictionary<string, string>(cachedValues)
        {
            ["Key2"] = "FactoryValue2"
        };

        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task BatchGetOrCreateAsync_ShouldCacheNewValues_WhenFactoryFunctionIsCalled()
    {
        var keys = new List<string> { "Key1", "Key2" };
        var factoryValues = new Dictionary<string, string>
        {
            { "Key1", "FactoryValue1" },
            { "Key2", "FactoryValue2" }
        };

        foreach (var key in keys)
        {
            var cacheKey = key;
            distributedCache.GetAsync(cacheKey, Arg.Any<CancellationToken>()).Returns((byte[]?)null);
        }

        await cacheService.BatchGetOrCreate(
            keys,
            (_, _) => Task.FromResult(factoryValues),
            null,
            CancellationToken.None
        );

        foreach (var key in keys)
        {
            var cacheKey = key;
            await distributedCache.Received(1).SetAsync(
                cacheKey,
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>()
            );
        }
    }
}
