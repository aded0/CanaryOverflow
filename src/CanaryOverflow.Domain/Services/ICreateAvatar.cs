using System;
using System.Threading.Tasks;

namespace CanaryOverflow.Domain.Services;

public interface ICreateAvatar
{
    /// <summary>
    /// CreateAsync is combination of generation and upload avatar.
    /// </summary>
    /// <param name="userId">Seed of color.</param>
    /// <param name="text">Text to write in.</param>
    /// <returns>Returns id of uploaded avatar.</returns>
    Task<Guid> CreateAsync(Guid userId, string text);
}
