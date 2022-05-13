using System;
using System.Threading.Tasks;
using CanaryOverflow.Domain.Services;

namespace CanaryOverflow.Domain.UnitTests.Services;

public class FalseTagService : ITagService
{
    public Task<bool> IsExistsAsync(Guid id)
    {
        return Task.FromResult(false);
    }

    public Task<bool> IsExistsAsync(string name)
    {
        return Task.FromResult(false);
    }
}
