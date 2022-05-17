﻿using System;
using System.IO;
using System.Threading.Tasks;

namespace CanaryOverflow.Domain.Services;

public interface IAvatarService
{
    Stream Create(Guid userId, string text);
    Task<Guid> UploadAsync(Stream stream);
    Task DeleteAsync(Guid id);
}
