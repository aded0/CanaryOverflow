using System;
using System.Threading.Tasks;
using CanaryOverflow.Domain.Services;

namespace CanaryOverflow.Persistence.IntegrationTests;

public class DummyTagService : ITagService
{
    public Task<bool> IsExistsAsync(Guid id)
    {
        return Task.FromResult(true);
    }

    public Task<bool> IsExistsAsync(string name)
    {
        return Task.FromResult(false);
    }
}
