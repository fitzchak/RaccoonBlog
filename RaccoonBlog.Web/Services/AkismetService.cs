using System;
using System.Configuration;
using Joel.Net;
using RaccoonBlog.Web.Models;

namespace RaccoonBlog.Web.Services
{
	public static class AkismetService
	{
		private static string akismetKey;
		private static string AkismetKey
		{
			get
			{
				if (string.IsNullOrWhiteSpace(akismetKey))
				{
					using (var session = MvcApplication.DocumentStore.OpenSession())
					{
						akismetKey = session.Load<BlogConfig>("Blog/Config").AkismetKey;
					}
				}
				return akismetKey;
			}
		}

		private static string BlogUrl
		{
			get { return ConfigurationManager.AppSettings["MainUrl"]; }
		}

		public static bool CheckForSpam(PostComments.Comment comment)
		{
#if DEBUG 
			return false;
#endif
			var api = new Akismet(AkismetKey, BlogUrl, comment.UserAgent);
			if (!api.VerifyKey()) throw new Exception("Akismet API key invalid.");

			var akismetComment = new AkismetComment
			{
				Blog = BlogUrl,
				UserIp = comment.UserHostAddress,
				UserAgent = comment.UserAgent,
				CommentContent = comment.Body,
				CommentType = "comment",
				CommentAuthor = comment.Author,
				CommentAuthorEmail = comment.Email,
				CommentAuthorUrl = comment.Url,
			};

			//Check if Akismet thinks this comment is spam. Returns TRUE if spam.
			return api.CommentCheck(akismetComment);
		}

		public static void MarkHam(PostComments.Comment comment)
		{
			var api = new Akismet(AkismetKey, BlogUrl, comment.UserAgent);
			if (!api.VerifyKey()) throw new Exception("Akismet API key invalid.");

			var akismetComment = new AkismetComment
			{
				Blog = BlogUrl,
				UserIp = comment.UserHostAddress,
				UserAgent = comment.UserAgent,
				CommentContent = comment.Body,
				CommentType = "comment",
				CommentAuthor = comment.Author,
				CommentAuthorEmail = comment.Email,
				CommentAuthorUrl = comment.Url,
			};
#if !DEBUG
			api.SubmitHam(akismetComment);
#endif
		}

		public static void MarkSpam(PostComments.Comment comment)
		{
			var api = new Akismet(AkismetKey, BlogUrl, comment.UserAgent);
			if (!api.VerifyKey()) throw new Exception("Akismet API key invalid.");

			var akismetComment = new AkismetComment
			{
				Blog = BlogUrl,
				UserIp = comment.UserHostAddress,
				UserAgent = comment.UserAgent,
				CommentContent = comment.Body,
				CommentType = "comment",
				CommentAuthor = comment.Author,
				CommentAuthorEmail = comment.Email,
				CommentAuthorUrl = comment.Url,
			};

#if !DEBUG
			api.SubmitSpam(akismetComment);
#endif
		}
	}
}
