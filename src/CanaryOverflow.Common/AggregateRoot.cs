using System.Linq.Expressions;

namespace CanaryOverflow.Common;

public abstract class AggregateRoot<TKey, TAggregate> : Entity<TKey>
    where TAggregate : AggregateRoot<TKey, TAggregate>
{
    private static readonly Func<TAggregate> Constructor;

    static AggregateRoot()
    {
        var constructorExpression = Expression.New(typeof(TAggregate));
        var lambdaExpression = Expression.Lambda<Func<TAggregate>>(constructorExpression);
        Constructor = lambdaExpression.Compile();
    }

    public static TAggregate Create(IReadOnlyCollection<IDomainEvent> events)
    {
        if (events.Count < 1)
            throw new ArgumentException("Unable to create instance without events", nameof(events));

        var aggregate = Constructor.Invoke();

        foreach (var @event in events)
        {
            aggregate.When(@event);
            aggregate.Version++;
        }

        return aggregate;
    }


    private readonly Queue<IDomainEvent> _events;

    protected AggregateRoot()
    {
        _events = new Queue<IDomainEvent>();
    }

    public int Version { get; protected set; }

    protected abstract void When(IDomainEvent @event);

    protected void Append(IDomainEvent @event)
    {
        _events.Enqueue(@event);

        Version++;

        When(@event);
    }

    public IReadOnlyCollection<IDomainEvent> GetUncommittedEvents() => _events;
    public void ClearUncommittedEvents() => _events.Clear();
}
