using System;
using System.Threading.Tasks;
using CanaryOverflow.Domain.Services;
using CanaryOverflow.Domain.TagAggregate;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CanaryOverflow.Persistence.IntegrationTests;

public class TagAggregateTests : IClassFixture<AggregateRepositoryProvider<Guid, Tag>>
{
    private readonly AggregateRepositoryProvider<Guid, Tag> _aggregateRepositoryProvider;

    public TagAggregateTests(AggregateRepositoryProvider<Guid, Tag> aggregateRepositoryProvider)
    {
        _aggregateRepositoryProvider = aggregateRepositoryProvider;
    }

    [Fact]
    [Trait("Category", "Tag/Create")]
    public async Task Create_tag_aggregate()
    {
        using var scope = _aggregateRepositoryProvider.Services.CreateScope();
        var aggregateRepository = scope.ServiceProvider.GetRequiredService<IAggregateRepository<Guid, Tag>>();
        var tagService = scope.ServiceProvider.GetRequiredService<ITagService>();

        var tag = await Tag.Create("javascript", "this is prototype oriented language", tagService);
        tag.UpdateName("jabbascript");
        tag.UpdateDescription("suck everywhere");

        await aggregateRepository.SaveAsync(tag);

        var tagRestored = await aggregateRepository.FindAsync(tag.Id);
    }
}
