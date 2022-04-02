using System;

namespace CanaryOverflow.Domain.UserAggregate;

// public class User : IdentityUser<Guid>
// {
//     public override bool Equals(object obj)
//     {
//         if (obj is not User other)
//             return false;
//
//         if (ReferenceEquals(this, other))
//             return true;
//             
//         if (IsTransient() || other.IsTransient())
//             return false;
//
//         return Id.Equals(other.Id);
//     }
//
//     private bool IsTransient()
//     {
//         return Id.Equals(Guid.Empty);
//     }
//
//     public static bool operator ==(User a, User b)
//     {
//         if (a is null && b is null)
//             return true;
//
//         if (a is null || b is null)
//             return false;
//
//         return a.Equals(b);
//     }
//
//     public static bool operator !=(User a, User b)
//     {
//         return !(a == b);
//     }
//
//     public override int GetHashCode()
//     {
//         return Id.GetHashCode();
//     }
// }
