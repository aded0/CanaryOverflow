using System;
using System.Threading.Tasks;
using CanaryOverflow.Domain.ProfileAggregate;
using CanaryOverflow.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CanaryOverflow.Persistence.IntegrationTests;

public class ProfileAggregateTests : IClassFixture<AggregateRepositoryProvider<Guid, Profile>>
{
    private readonly AggregateRepositoryProvider<Guid, Profile> _aggregateRepositoryProvider;

    public ProfileAggregateTests(AggregateRepositoryProvider<Guid, Profile> aggregateRepositoryProvider)
    {
        _aggregateRepositoryProvider = aggregateRepositoryProvider;
    }

    [Fact]
    [Trait("Category", "Profile/Create")]
    public async Task Create_profile_aggregate()
    {
        using var scope = _aggregateRepositoryProvider.Services.CreateScope();
        var aggregateRepository = scope.ServiceProvider.GetRequiredService<IAggregateRepository<Guid, Profile>>();
        var assetsService = scope.ServiceProvider.GetRequiredService<IAssetsService>();

        var profile = await Profile.Create("my display name", Guid.NewGuid(), null, assetsService);

        await aggregateRepository.SaveAsync(profile);
    }
}
