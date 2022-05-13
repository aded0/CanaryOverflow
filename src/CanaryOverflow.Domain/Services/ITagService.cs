using System;
using System.Threading.Tasks;

namespace CanaryOverflow.Domain.Services;

public interface ITagService
{
    Task<bool> IsExistsAsync(Guid id);
    Task<bool> IsExistsAsync(string name);
}
