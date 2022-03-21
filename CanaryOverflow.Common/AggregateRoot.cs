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

    public static TAggregate Create(IReadOnlyCollection<object> events)
    {
        if (events.Count < 1)
            throw new ArgumentException("Unable to create instance without events", nameof(events));

        var aggregate = Constructor.Invoke();

        foreach (var @event in events)
            aggregate.When(@event);

        aggregate.Version = events.Count;
        return aggregate;
    }


    private readonly Queue<object> _events;

    protected AggregateRoot()
    {
        _events = new Queue<object>();
    }

    public int Version { get; private set; }

    protected abstract void When(object @event);

    protected void Append(object @event)
    {
        _events.Enqueue(@event);

        Version++;

        When(@event);
    }

    public IReadOnlyCollection<object> GetUncommittedEvents() => _events;
    public void ClearUncommittedEvents() => _events.Clear();
}
