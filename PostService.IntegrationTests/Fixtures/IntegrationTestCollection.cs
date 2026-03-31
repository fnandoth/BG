using Xunit;

namespace PostService.IntegrationTests.Fixtures;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class IntegrationTestCollection : ICollectionFixture<IntegrationTestFixture>
{
    public const string Name = "post-service-integration";
}
