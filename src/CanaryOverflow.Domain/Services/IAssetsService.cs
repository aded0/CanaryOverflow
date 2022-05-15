using System;
using System.Threading.Tasks;

namespace CanaryOverflow.Domain.Services;

public interface IAssetsService
{
    Task<bool> IsAvatarExistsAsync(Guid id);
}
