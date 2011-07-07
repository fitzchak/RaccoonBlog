// -----------------------------------------------------------------------
//  <copyright file="CommentsLogin.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;
using RaccoonBlog.Web.Helpers;

namespace RaccoonBlog.Web.Controllers
{
	public class SocialLoginController : AbstractController
	{
		private static readonly OpenIdRelyingParty openid = new OpenIdRelyingParty();

		public ActionResult Authenticate(string identifier)
		{
			string returnUrl = Url.RouteUrl("default");
			if (Request.UrlReferrer != null)
				returnUrl = Request.UrlReferrer.ToString();

			var response = openid.GetResponse();
			if (response == null)
			{
				Identifier id;
				if (Identifier.TryParse(identifier, out id) == false)
					ModelState.AddModelError("identifier", "The specified login identifier is invalid");

				if (ModelState.IsValid == false)
				{
					if (Request.IsAjaxRequest())
						return Json(new { Success = false, message = ModelState.GetFirstErrorMessage() });

					TempData["Message"] = ModelState.GetFirstErrorMessage();
					return Redirect(returnUrl);
				}

				try
				{
					var request = openid.CreateRequest(identifier);
					request.AddExtension(new ClaimsRequest { });
					return request.RedirectingResponse.AsActionResult();
				}
				catch (ProtocolException ex)
				{
					if (Request.IsAjaxRequest())
						return Json(new { message = ex.Message });
					TempData["Message"] = ex.Message;
					return Redirect(returnUrl);
				}
			}
			switch (response.Status)
			{
				case AuthenticationStatus.Authenticated:
					return Redirect(returnUrl);
				case AuthenticationStatus.Canceled:
					TempData["Message"] = "Canceled at provider";
					return Redirect(returnUrl);
				case AuthenticationStatus.Failed:
					TempData["Message"] = response.Exception.Message;
					return Redirect(returnUrl);
			}
			return new EmptyResult();
		}
	}
}