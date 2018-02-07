using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace RaccoonBlog.Models.Account
{
    public class UserStore: 
        IUserStore<ApplicationUser>,
        IUserLoginStore<ApplicationUser>, 
        IUserClaimStore<ApplicationUser>, 
        IUserRoleStore<ApplicationUser>,
        IUserPasswordStore<ApplicationUser>, 
        IUserSecurityStampStore<ApplicationUser>, 
        IUserEmailStore<ApplicationUser>, 
        IUserLockoutStore<ApplicationUser>,
        IUserTwoFactorStore<ApplicationUser>, 
        IUserPhoneNumberStore<ApplicationUser>,
        IUserAuthenticatorKeyStore<ApplicationUser>
    {
        private bool _disposed;
        private readonly IAsyncDocumentSession _session;

        public UserStore()
        {
            // TODO: Create just once session to users database per request.
            // We can do this by moving users to the same database as Admin or Orders.
            _session = DocumentStoreHolder.OpenAsyncSession();
        }

        public void Dispose()
        {
            _disposed = true;
            // TODO: Save the session
            _session.Dispose();
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            user.UserName = normalizedName.ToLowerInvariant();
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);
            if (string.IsNullOrEmpty(user.Id))
                user.Id = "ApplicationUsers/" + user.Email;

            await _session.StoreAsync(user, cancellationToken);
            await _session.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            return Task.FromResult(IdentityResult.Success);
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            _session.Delete(user);
            await _session.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _session.LoadAsync<ApplicationUser>(userId, cancellationToken);
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            ThrowIfDisposedOrCancelled(cancellationToken);

            if (string.IsNullOrEmpty(normalizedUserName))
                throw new ArgumentNullException(nameof(normalizedUserName));

            var id = "ApplicationUsers/" + normalizedUserName;
            return await FindByIdAsync(id, cancellationToken);
        }

        private void ThrowIfNullDisposedCancelled(ApplicationUser user, CancellationToken token)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ThrowIfDisposedOrCancelled(token);
        }

        private void ThrowIfDisposedOrCancelled(CancellationToken token)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            token.ThrowIfCancellationRequested();
        }

        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            return Task.FromResult<IList<UserLoginInfo>>(user.Logins);
        }

        public Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            IList<Claim> result = user.Claims
                .Select(c => new Claim(c.ClaimType, c.ClaimValue))
                .ToList();
            return Task.FromResult(result);
        }

        public Task AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            user.Claims.AddRange(claims.Select(c => new IdentityUserClaim { ClaimType = c.Type, ClaimValue = c.Value }));
            return Task.CompletedTask;
        }

        public async Task ReplaceClaimAsync(ApplicationUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);
            
            var indexOfClaim = user.Claims.FindIndex(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value);
            if (indexOfClaim != -1)
            {
                user.Claims.RemoveAt(indexOfClaim);
                await AddClaimsAsync(user, new[] { newClaim }, cancellationToken);
            }
        }

        public Task RemoveClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            user.Claims.RemoveAll(identityClaim => claims.Any(c => c.Type == identityClaim.ClaimType && c.Value == identityClaim.ClaimValue));
            return Task.CompletedTask;
        }

        public async Task<IList<ApplicationUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            ThrowIfDisposedOrCancelled(cancellationToken);
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            return await _session.Query<ApplicationUser>()
                .Where(u => u.Claims.Any(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value))
                .ToListAsync(cancellationToken);
        }

        public async Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            var roleNameLowered = roleName.ToLower();
            user.Roles.Add(roleNameLowered);

            // See if we have an IdentityRole with that.
            var roleId = "IdentityRoles/" + roleNameLowered;
            var existingRoleOrNull = await _session.LoadAsync<IdentityRole>(roleId, cancellationToken);
            if (existingRoleOrNull == null)
            {
                ThrowIfDisposedOrCancelled(cancellationToken);
                existingRoleOrNull = new IdentityRole(roleNameLowered);
                await _session.StoreAsync(existingRoleOrNull, roleId, cancellationToken);
            }

            existingRoleOrNull.Users.Add(user.Id);
        }

        public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            user.Roles.Remove(roleName);

            var roleId = "IdentityRoles/" + roleName.ToLower();
            var roleOrNull = await _session.LoadAsync<IdentityRole>(roleId, cancellationToken);
            roleOrNull?.Users.Remove(user.Id);
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            return Task.FromResult<IList<string>>(user.Roles.ToList());
        }

        public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException(nameof(roleName));

            return Task.FromResult(user.Roles.Contains(roleName));
        }

        public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            ThrowIfDisposedOrCancelled(cancellationToken);
            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException(nameof(roleName));

            return await _session.Query<ApplicationUser>()
                .Where(u => u.Roles.Contains(roleName))
                .Take(1024)
                .ToListAsync(cancellationToken);
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetSecurityStampAsync(ApplicationUser user, string stamp, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            user.SecurityStamp = stamp;
            return Task.CompletedTask;
        }

        public Task<string> GetSecurityStampAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
        {
            ThrowIfDisposedOrCancelled(cancellationToken);
            user.Email = email ?? throw new ArgumentNullException(nameof(email));
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            ThrowIfDisposedOrCancelled(cancellationToken);

            return await _session.Query<ApplicationUser>()
                .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
        }

        public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            return Task.FromResult(user.Email);
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);
            if (string.IsNullOrEmpty(normalizedEmail))
                throw new ArgumentNullException(nameof(normalizedEmail));

            user.Email = normalizedEmail.ToLowerInvariant();
            return Task.CompletedTask;
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            return Task.FromResult(user.LockoutEndDate);
        }

        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            user.LockoutEndDate = lockoutEnd;
            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            user.AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        public Task<int> GetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task<string> GetPhoneNumberAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            return Task.FromResult(user.IsPhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            ThrowIfNullDisposedCancelled(user, cancellationToken);

            user.IsPhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetAuthenticatorKeyAsync(ApplicationUser user, string key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetAuthenticatorKeyAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
