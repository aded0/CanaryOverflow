using System;
using System.Threading.Tasks;

namespace CanaryOverflow.Domain.Services;

public interface IProfileService
{
    Task<bool> IsExistsAsync(Guid id);
}
