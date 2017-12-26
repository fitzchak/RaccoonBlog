/*using System.Security.Claims;
using System.Security.Principal;

using HibernatingRhinos.Loci.Common.Models;

using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace RaccoonBlog.Web.Helpers
{
	public class SignInHelper
	{
		private readonly IAuthenticationManager authenticationManager;

		public SignInHelper(IAuthenticationManager authenticationManager)
		{
			this.authenticationManager = authenticationManager;
		}

		public void SignIn(LogOnModel logOn, bool isPersistent)
		{
			authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);

			var identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
			identity.AddClaim(new Claim(ClaimTypes.Email, logOn.Login));

			if (logOn.RememberMe)
			{
				var rememberBrowserIdentity =
					authenticationManager.CreateTwoFactorRememberBrowserIdentity(logOn.Login);

				authenticationManager.SignIn(
					new AuthenticationProperties { IsPersistent = isPersistent },
					identity,
					rememberBrowserIdentity);
			}
			else
			{
				authenticationManager.SignIn(
					new AuthenticationProperties { IsPersistent = isPersistent },
					identity);
			}
		}

		public void SignOut()
		{
			authenticationManager.SignOut();
		}
	}
}*/