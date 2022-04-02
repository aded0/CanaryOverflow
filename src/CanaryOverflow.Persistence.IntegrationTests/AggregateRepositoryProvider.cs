using System;
using CanaryOverflow.Common;
using CanaryOverflow.Domain.QuestionAggregate;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace CanaryOverflow.Persistence.IntegrationTests;

[UsedImplicitly]
public sealed class AggregateRepositoryProvider<TKey, TAggregate> : IDisposable
    where TAggregate : AggregateRoot<TKey, TAggregate>
{
    private const string EventStoreDbConnectionString = "esdb://192.168.1.4:2113";

    public AggregateRepositoryProvider()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection
            .AddEventStoreClient(EventStoreDbConnectionString)
            .AddDomainEventTypesCache()
            .AddScoped(typeof(AggregateRepository<TKey, TAggregate>), typeof(AggregateRepository<Guid, Question>));

        Services = serviceCollection.BuildServiceProvider();
    }

    public ServiceProvider Services { get; }

    public void Dispose() => Services.Dispose();
}
