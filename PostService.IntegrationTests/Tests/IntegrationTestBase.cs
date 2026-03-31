using PostService.IntegrationTests.Fixtures;
using Xunit;

namespace PostService.IntegrationTests.Tests;

[Collection(IntegrationTestCollection.Name)]
public abstract class IntegrationTestBase(IntegrationTestFixture fixture) : IAsyncLifetime
{
    protected IntegrationTestFixture Fixture { get; } = fixture;

    public virtual Task InitializeAsync()
    {
        return Fixture.ResetStateAsync();
    }

    public virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
