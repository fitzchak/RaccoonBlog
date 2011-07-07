// -----------------------------------------------------------------------
//  <copyright file="CommentsLogin.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Web.Mvc;
using Brickred.SocialAuth.NET.Core.BusinessObjects;

namespace RaccoonBlog.Web.Controllers
{
	public class SocialLoginController : AbstractController
	{
		public ActionResult Login(PROVIDER_TYPE provider)
		{
			string returnUrl = string.Empty;
			if (Request.UrlReferrer != null)
				returnUrl = Request.UrlReferrer.ToString();

			var socialAuthUser = new SocialAuthUser(provider);
			if (socialAuthUser.IsLoggedIn == false)
				socialAuthUser.Login(returnUrl);

			return Redirect(returnUrl);
		}	
	}
}