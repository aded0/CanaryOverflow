using System.Text.Json;
using CanaryOverflow.Common;
using EventStore.Client;
using Microsoft.Toolkit.HighPerformance;

namespace CanaryOverflow.Infrastructure;

public class AggregateRepository<TKey, TAggregate> : IAggregateRepository<TKey, TAggregate>
    where TAggregate : AggregateRoot<TKey, TAggregate>
{
    private static readonly string StreamPrefix = typeof(TAggregate).Name;
    private static string GetStreamName(TKey key) => $"{StreamPrefix}-{key}";


    private readonly EventStoreClient _eventStoreClient;
    private readonly IDomainEventTypesCache _domainEventTypesCache;

    public AggregateRepository(EventStoreClient eventStoreClient, IDomainEventTypesCache domainEventTypesCache)
    {
        _eventStoreClient = eventStoreClient;
        _domainEventTypesCache = domainEventTypesCache;
    }

    public async Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
    {
        if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

        var events = aggregate.GetUncommittedEvents();

        if (events.Count < 1) return;

        var streamName = GetStreamName(aggregate.Id);
        var expectedRevision = StreamRevision.FromInt64(aggregate.Version - events.Count - 1);
        var eventData = events.Select(AsEventData);

        await _eventStoreClient.AppendToStreamAsync(streamName, expectedRevision, eventData,
            cancellationToken: cancellationToken);

        aggregate.ClearUncommittedEvents();
    }

    private static EventData AsEventData(IDomainEvent @event)
    {
        var data = JsonSerializer.SerializeToUtf8Bytes(@event);

        return new EventData(
            eventId: Uuid.NewUuid(),
            type: @event.GetType().Name,
            data: new ReadOnlyMemory<byte>(data)
        );
    }

    public async Task<TAggregate> FindAsync(TKey key, CancellationToken cancellationToken = default)
    {
        var streamName = GetStreamName(key);

        var events = await _eventStoreClient
            .ReadStreamAsync(Direction.Forwards, streamName, StreamPosition.Start, cancellationToken: cancellationToken)
            .Select(AsDomainEventAsync)
            .NotNull()
            .ToListAsync(cancellationToken);

        return AggregateRoot<TKey, TAggregate>.Create(events);
    }

    private IDomainEvent? AsDomainEventAsync(ResolvedEvent @event)
    {
        var data = @event.Event.Data.AsStream();
        var type = _domainEventTypesCache[@event.Event.EventType];
        return JsonSerializer.Deserialize(data, type) as IDomainEvent;
    }
}
