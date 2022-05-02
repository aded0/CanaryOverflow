using System;
using System.Threading.Tasks;
using CanaryOverflow.Domain.Services;

namespace CanaryOverflow.Persistence.IntegrationTests;

public class DummyAssetsService : IAssetsService
{
    public Task<bool> IsAvatarExists(Guid id)
    {
        return Task.FromResult(true);
    }
}
