namespace CanaryOverflow.Persistence;

public interface IDomainEventTypesCache
{
    Type this[string typeName] { get; }
}
