using System;
using System.Threading.Tasks;
using CanaryOverflow.Domain.Services;

namespace CanaryOverflow.Domain.UnitTests.Services;

public class TrueTagService : ITagService
{
    public Task<bool> IsExistsAsync(Guid id)
    {
        return Task.FromResult(true);
    }

    public Task<bool> IsExistsAsync(string name)
    {
        return Task.FromResult(true);
    }
}
