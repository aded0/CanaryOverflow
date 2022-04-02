using CanaryOverflow.Common;

namespace CanaryOverflow.Persistence;

public interface IAggregateRepository<in TKey, TAggregate>
    where TAggregate : AggregateRoot<TKey, TAggregate>
{
    Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken = default);
    Task<TAggregate> FindAsync(TKey key, CancellationToken cancellationToken = default);
}
