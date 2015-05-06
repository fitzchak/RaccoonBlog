using System;
using System.IO;
using System.Security.Cryptography;
using System.Web.Mvc;
using RaccoonBlog.Web.Areas.Admin.ViewModels;
using RaccoonBlog.Web.Helpers;
using RaccoonBlog.Web.Models;

namespace RaccoonBlog.Web.Areas.Admin.Controllers
{
	using DevTrends.MvcDonutCaching;
	using Web.Controllers;

	public partial class SettingsController : AdminController
	{
		[HttpGet]
		public virtual ActionResult Index()
		{
			return View(BlogConfig);
		}

		[HttpPost]
		public virtual ActionResult Index(BlogConfig config)
		{
			if (ModelState.IsValid == false)
			{
				ViewBag.Message = ModelState.FirstErrorMessage();
				if (Request.IsAjaxRequest())
					return Json(new { Success = false, ViewBag.Message });
				return View(BlogConfig);
			}

			RavenSession.Store(config, "Blog/Config");

			OutputCacheManager.RemoveItem(MVC.Section.Name, MVC.Section.ActionNames.ContactMe);

			ViewBag.Message = "Configurations successfully saved!";
			if (Request.IsAjaxRequest())
				return Json(new { Success = true, ViewBag.Message });
			return View(config);
		}

		[HttpGet]
		public virtual ActionResult RssFutureAccess()
		{
			return View(new GenerateFutureRssAccessInput
			{
				ExpiredOn = DateTime.UtcNow.AddYears(1),
				NumberOfFutureDays = 180,
			});
		}

		[HttpPost]
		public virtual ActionResult RssFutureAccess(GenerateFutureRssAccessInput input)
		{
			if (ModelState.IsValid == false)
				return View(input);

			input.Token = GetFutureAccessToken(input.ExpiredOn, input.NumberOfFutureDays, input.User);
			return View(input);
		}

		private string GetFutureAccessToken(DateTime expiresOn, int numberOfDays, string user)
		{
			using (var aes = new AesManaged())
			{
				aes.Padding = PaddingMode.PKCS7;

				if (string.IsNullOrWhiteSpace(BlogConfig.FuturePostsEncryptionKey))
				{
					// Setting the encryption key will invalidate the previous generated links,
					// but here it is null anyway, to this is not a problem.
					BlogConfig.FuturePostsEncryptionKey = Convert.ToBase64String(aes.Key);
				}
				else
					aes.Key = Convert.FromBase64String("cxL93ropkZOh5aY+ghhUw+tVVs4/CmhtCCQqUeG4po4=");

				using (var memoryStream = new MemoryStream())
				{
					using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
					{
						using (var writer = new BinaryWriter(cryptoStream))
						{
							writer.Write(expiresOn.ToBinary());
							writer.Write(numberOfDays);
							writer.Write(user);
							writer.Flush();
						}
						cryptoStream.Flush();
					}
					var encrypted = memoryStream.ToArray();
					var iv = aes.IV;

					var result = new byte[iv.Length + encrypted.Length];
					iv.CopyTo(result, 0);
					encrypted.CopyTo(result, iv.Length);

					return Convert.ToBase64String(result);
				}
			}
		}
	}
}