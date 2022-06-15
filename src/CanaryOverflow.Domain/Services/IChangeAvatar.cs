using System;
using System.IO;
using System.Threading.Tasks;

namespace CanaryOverflow.Domain.Services;

public interface IChangeAvatar
{
    /// <summary>
    /// ChangeAsync is combination of upload and delete avatar.
    /// </summary>
    /// <param name="newAvatarData">Stream with new avatar data.</param>
    /// <param name="previousAvatarId">Avatar's id to remove.</param>
    /// <returns>Returns id of newly uploaded avatar</returns>
    Task<Guid> ChangeAsync(Stream newAvatarData, Guid previousAvatarId);
}
