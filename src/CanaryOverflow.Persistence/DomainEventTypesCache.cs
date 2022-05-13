using System.Reflection;
using CanaryOverflow.Common;

namespace CanaryOverflow.Persistence;

public class DomainEventTypesCache : IDomainEventTypesCache
{
    private readonly Dictionary<string, Type> _types;

    public DomainEventTypesCache(IEnumerable<Assembly> assemblies)
    {
        var domainEventType = typeof(IDomainEvent);
        _types = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsInterface)
            .Where(t => domainEventType.IsAssignableFrom(t))
            .ToDictionary(t => t.Name);
    }

    public Type this[string typeName] => _types[typeName];
}
