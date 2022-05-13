using Microsoft.Extensions.DependencyInjection;

namespace CanaryOverflow.Persistence;

// ReSharper disable once InconsistentNaming
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddDomainEventTypesCache(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddSingleton<IDomainEventTypesCache>(
            new DomainEventTypesCache(AppDomain.CurrentDomain.GetAssemblies()));
    }
}
