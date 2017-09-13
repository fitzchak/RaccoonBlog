using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.MicrosoftAccount;
using Microsoft.Owin.Security.Twitter;

using Owin;

using RaccoonBlog.Web.Helpers;

namespace RaccoonBlog.Web
{
	public partial class Startup
	{
		public void ConfigureAuth(IAppBuilder app)
		{
			// Enable the application to use a cookie to store information for the signed in user
			// and to use a cookie to temporarily store information about a user logging in with a third party login provider
			// Configure the sign in cookie
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				LoginPath = new PathString("/admin/Login")
			});

			app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

			AddGoogle(app);
			AddMicrosoftAccount(app);
			//AddTwitterAccount(app);
			AddFacebookAccount(app);
		}

		private static void AddFacebookAccount(IAppBuilder app)
		{
			var keys = ConfigurationHelper.FacebookOAuthKeys;
			if (keys == null)
				return;

			var facebookAuthenticationOptions = new FacebookAuthenticationOptions
						{
							AppId = keys.Item1,
							AppSecret = keys.Item2
						};
			facebookAuthenticationOptions.Scope.Add("email");

			app.UseFacebookAuthentication(facebookAuthenticationOptions);
		}

		private static void AddTwitterAccount(IAppBuilder app)
		{
			var keys = ConfigurationHelper.TwitterOAuthKeys;
			if (keys == null)
				return;

			app.UseTwitterAuthentication(new TwitterAuthenticationOptions
			{
				ConsumerKey = keys.Item1,
				ConsumerSecret = keys.Item2
			});
		}

		private static void AddGoogle(IAppBuilder app)
		{
			var keys = ConfigurationHelper.GoogleOAuthKeys;
			if (keys == null)
				return;

			app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
			{
				ClientId = keys.Item1,
				ClientSecret = keys.Item2,
			});
		}

		private static void AddMicrosoftAccount(IAppBuilder app)
		{
			var keys = ConfigurationHelper.MicrosoftOAuthKeys;
			if (keys == null)
				return;

			var microsoftAccountAuthenticationOptions = new MicrosoftAccountAuthenticationOptions
			{
				ClientId = keys.Item1,
				ClientSecret = keys.Item2,
			};
			microsoftAccountAuthenticationOptions.Scope.Add("https://graph.microsoft.com/user.read");
			microsoftAccountAuthenticationOptions.Scope.Add("https://graph.microsoft.com/mail.read");

			app.UseMicrosoftAccountAuthentication(microsoftAccountAuthenticationOptions);
		}
	}
}