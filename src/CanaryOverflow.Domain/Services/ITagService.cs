using System.Threading.Tasks;

namespace CanaryOverflow.Domain.Services;

public interface ITagService
{
    Task<bool> IsExistsAsync(string name);
}
