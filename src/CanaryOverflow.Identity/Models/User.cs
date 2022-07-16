using Microsoft.AspNetCore.Identity;

namespace CanaryOverflow.Identity.Models;

public sealed class User : IdentityUser<Guid>
{
    public User(string userName, string email) : base(userName)
    {
        Email = email;
    }
}
