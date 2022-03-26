namespace CanaryOverflow.Infrastructure;

public interface IDomainEventTypesCache
{
    Type this[string typeName] { get; }
}
