// -----------------------------------------------------------------------
//  <copyright file="CommentsLogin.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;
using RaccoonBlog.Web.Helpers;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Controllers
{
	public class SocialLoginController : RaccoonController
	{
		public ActionResult Authenticate(string url, string returnUrl)
		{
			if (string.IsNullOrWhiteSpace(returnUrl))
				returnUrl = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : Url.RouteUrl("default");

			using (var openIdRelyingParty = new OpenIdRelyingParty())
			{
				var response = openIdRelyingParty.GetResponse();
				if (response == null)
				{
					return HandleNullResponse(url, returnUrl);
				}

				switch (response.Status)
				{
					case AuthenticationStatus.Authenticated:
						var claimedIdentifier = response.ClaimedIdentifier.ToString();
						var commenter = RavenSession.Query<Commenter>()
						                	.Where(c => c.OpenId == claimedIdentifier)
						                	.FirstOrDefault() ?? new Commenter
						                	{
						                		Key = Guid.NewGuid(),
						                		OpenId = claimedIdentifier,
						                	};

						SetCommenterValuesFromOpenIdResponse(response, commenter);
						
						CommenterUtil.SetCommenterCookie(Response, commenter.Key.MapTo<string>());
						RavenSession.Store(commenter);

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

		private static void SetCommenterValuesFromOpenIdResponse(IAuthenticationResponse response, Commenter commenter)
		{
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
			}
		}

		private ActionResult HandleNullResponse(string url, string returnUrl)
		{
			Identifier id;
			if (Identifier.TryParse(url, out id) == false)
				ModelState.AddModelError("identifier", "The specified login identifier is invalid");

			if (ModelState.IsValid == false)
			{
				if (Request.IsAjaxRequest())
					return Json(new {Success = false, message = ModelState.GetFirstErrorMessage()});

				TempData["Message"] = ModelState.GetFirstErrorMessage();
				return Redirect(returnUrl);
			}

			try
			{
				var theReturnUrlForOpenId = new UriBuilder(Url.AbsoluteAction("Authenticate"))
				{
					Query = "returnUrl=" + Uri.EscapeUriString(returnUrl)
				}.Uri;

				using (var openIdRelyingParty = new OpenIdRelyingParty())
				{
					var request = openIdRelyingParty.CreateRequest(url, Realm.AutoDetect, theReturnUrlForOpenId);
					request.AddExtension(new FetchRequest // requires for Google 
					{
						Attributes =
							{
								new AttributeRequest(WellKnownAttributes.Name.First, true),
								new AttributeRequest(WellKnownAttributes.Name.Last, true),
								new AttributeRequest(WellKnownAttributes.Contact.Email, true),
								new AttributeRequest(WellKnownAttributes.Contact.Web.Blog, true),
								new AttributeRequest(WellKnownAttributes.Contact.Web.Homepage, true),
								new AttributeRequest(WellKnownAttributes.Name.FullName, false),
							}
					});
					request.AddExtension(new ClaimsRequest // other providers
					{
						Email = DemandLevel.Require,
						FullName = DemandLevel.Require,
						Nickname = DemandLevel.Require,
					});
					return request.RedirectingResponse.AsActionResult();
				}
			}
			catch (ProtocolException ex)
			{
				if (Request.IsAjaxRequest())
					return Json(new {message = ex.Message});
				TempData["Message"] = ex.Message;
				return Redirect(returnUrl);
			}
		}
	}
}
