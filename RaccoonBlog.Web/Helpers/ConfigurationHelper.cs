using System;
using System.Configuration;

namespace RaccoonBlog.Web.Helpers
{
	public static class ConfigurationHelper
	{
		private static Tuple<string, string> microsoftOAuthKeys;

		private static Tuple<string, string> googleOAuthKeys;

		private static Tuple<string, string> twitterOAuthKeys;

		private static Tuple<string, string> facebookOAuthKeys;

		public static Tuple<string, string> MicrosoftOAuthKeys
		{
			get
			{
				return microsoftOAuthKeys ?? (microsoftOAuthKeys = GetKeys("Microsoft", "ClientId", "ClientSecret"));
			}
		}

		public static Tuple<string, string> GoogleOAuthKeys
		{
			get
			{
				return googleOAuthKeys ?? (googleOAuthKeys = GetKeys("Google", "ClientId", "ClientSecret"));
			}
		}

		public static Tuple<string, string> TwitterOAuthKeys
		{
			get
			{
				//return twitterOAuthKeys ?? (twitterOAuthKeys = GetKeys("Twitter", "ConsumerKey", "ConsumerSecret"));
				return null;
			}
		}

		public static Tuple<string, string> FacebookOAuthKeys
		{
			get
			{
				return facebookOAuthKeys ?? (facebookOAuthKeys = GetKeys("Facebook", "AppId", "AppSecret"));
			}
		}

		private static Tuple<string, string> GetKeys(string provider, string idKey, string secretKey)
		{
			var keyPrefix = "Raccoon/OAuth/" + provider;
			var id = ConfigurationManager.AppSettings[keyPrefix + "/" + idKey];
			var secret = ConfigurationManager.AppSettings[keyPrefix + "/" + secretKey];

			if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(secret))
				return null;

			return new Tuple<string, string>(id, secret);
		}
	}
}