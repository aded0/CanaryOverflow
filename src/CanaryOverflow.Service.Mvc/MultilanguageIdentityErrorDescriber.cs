using CanaryOverflow.Service.Mvc.Resources;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace CanaryOverflow.Service.Mvc;

[UsedImplicitly]
public class MultilanguageIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = IdentityErrors.PasswordRequiresNonAlphanumeric
        };
    }

    public override IdentityError DuplicateUserName(string userName)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateUserName),
            Description = string.Format(IdentityErrors.DuplicateUserName, userName)
        };
    }

    public override IdentityError InvalidUserName(string userName)
    {
        return new IdentityError
        {
            Code = nameof(InvalidUserName),
            Description = string.Format(IdentityErrors.InvalidUserName, userName)
        };
    }

    //todo: localize the rest
}
