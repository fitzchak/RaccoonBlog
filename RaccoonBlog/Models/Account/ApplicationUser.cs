using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace RaccoonBlog.Models.Account
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser
    {
        /// <summary>
        /// Creates a new IdentityUser.
        /// </summary>
        public ApplicationUser()
        {
            Claims = new List<IdentityUserClaim>();
            Roles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Logins = new List<UserLoginInfo>();
        }

        /// <summary>
        /// The ID of the user.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The user name. Usually the same as the email.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The password hash.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// The security stamp.
        /// </summary>
        public string SecurityStamp { get; set; }

        /// <summary>
        /// The email of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The phone number.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Whether the user has confirmed their email address.
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Whether the user has confirmed their phone.
        /// </summary>
        public bool IsPhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Number of times sign in failed.
        /// </summary>
        public int AccessFailedCount { get; set; }

        /// <summary>
        /// Whether the user is locked out.
        /// </summary>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// When the user lock out is over.
        /// </summary>
        public DateTimeOffset? LockoutEndDate { get; set; }

        /// <summary>
        /// Whether 2-factor authentication is enabled.
        /// </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// The roles of the user. To modify the user's roles, use <see cref="UserManager{TUser}.AddToRoleAsync(TUser, string)"/> nad <see cref="UserManager{TUser}.RemoveFromRolesAsync(TUser, IEnumerable{string})"/>.
        /// </summary>
        public HashSet<string> Roles { get; private set; }

        /// <summary>
        /// The user's claims, for use in claims-based authentication.
        /// </summary>
        public List<IdentityUserClaim> Claims { get; private set; }

        /// <summary>
        /// The logins of the user.
        /// </summary>
        public List<UserLoginInfo> Logins { get; private set; }
    }

    public sealed class IdentityUserLogin
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Provider { get; set; }
        public string ProviderKey { get; set; }
    }

    public class IdentityUserClaim
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
