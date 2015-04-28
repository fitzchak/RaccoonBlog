// -----------------------------------------------------------------------
//  <copyright file="SocialController.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using RaccoonBlog.Web.Helpers;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Controllers
{
	public partial class SocialController : RaccoonController
	{

		public virtual ActionResult Login(string provider, string redirectUrl)
		{
			// Request a redirect to the external login provider
			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Social", new { ReturnUrl = redirectUrl }));
		}

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}


		private const string XsrfKey = "XsrfId";

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string returnUrl)
				: this(provider, returnUrl, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}

		private static void SetCommenterValuesFromResponse(ExternalLoginInfo response, Commenter commenter)
		{
			var claims = response.ExternalIdentity;

			if (string.IsNullOrWhiteSpace(commenter.Name))
			{
				var email = claims.FindFirst(ClaimTypes.Email);

			}
			
			//TODO: 
			/*
			var claimsResponse = response.GetExtension<ClaimsResponse>();
			if (claimsResponse != null)
			{
				if (string.IsNullOrWhiteSpace(commenter.Name) && string.IsNullOrWhiteSpace(claimsResponse.Nickname) == false)
					commenter.Name = claimsResponse.Nickname;
				else if (string.IsNullOrWhiteSpace(commenter.Name) && string.IsNullOrWhiteSpace(claimsResponse.FullName) == false)
					commenter.Name = claimsResponse.FullName;
				if (string.IsNullOrWhiteSpace(commenter.Email) && string.IsNullOrWhiteSpace(claimsResponse.Email) == false)
					commenter.Email = claimsResponse.Email;
			}
			var fetchResponse = response.GetExtension<FetchResponse>();
			if (fetchResponse != null) // let us try from the attributes
			{
				if (string.IsNullOrWhiteSpace(commenter.Email))
					commenter.Email = fetchResponse.GetAttributeValue(WellKnownAttributes.Contact.Email);
				if (string.IsNullOrWhiteSpace(commenter.Name))
				{
					commenter.Name = fetchResponse.GetAttributeValue(WellKnownAttributes.Name.FullName) ??
									 fetchResponse.GetAttributeValue(WellKnownAttributes.Name.First) + " " +
									 fetchResponse.GetAttributeValue(WellKnownAttributes.Name.Last);
				}

				if (string.IsNullOrWhiteSpace(commenter.Url))
				{
					commenter.Url = fetchResponse.GetAttributeValue(WellKnownAttributes.Contact.Web.Blog) ??
								fetchResponse.GetAttributeValue(WellKnownAttributes.Contact.Web.Homepage);
				}
			}*/
		}

		
		[AllowAnonymous]
		public virtual async Task<ActionResult> ExternalLoginCallback(string returnUrl)
		{
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
			if (loginInfo == null)
			{
				//TODO: better hadling (HandleNullResponse)
				return Redirect("homepage");
			}

			//TODO: handle falures

			var claimedIdentifier = loginInfo.Login.ProviderKey + "@" + loginInfo.Login.LoginProvider;
			var commenter = RavenSession.Query<Commenter>()
								.FirstOrDefault(c => c.OpenId == claimedIdentifier) ?? new Commenter
																					   {
																						   Key = Guid.NewGuid(),
																						   OpenId = claimedIdentifier,
																					   };

			SetCommenterValuesFromResponse(loginInfo, commenter);

			CommenterUtil.SetCommenterCookie(Response, commenter.Key.MapTo<string>());
			RavenSession.Store(commenter);

			return Redirect(returnUrl);

			/*
			// Sign in the user with this external login provider if the user already has a login
			var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
			switch (result)
			{
				case SignInStatus.Success:
					return RedirectToLocal(returnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
				case SignInStatus.Failure:
				default:
					// If the user does not have an account, then prompt the user to create an account
					ViewBag.ReturnUrl = returnUrl;
					ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
					return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
			}*/
			throw new NotImplementedException();
		}
	}
}
