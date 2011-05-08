using System;
using System.Configuration;
using Joel.Net;
using RaccoonBlog.Web.Models;
using Raven.Client;

namespace RaccoonBlog.Web.Services
{
    public class AskimetService
    {
    	private IDocumentSession session;
    	private string akismetKey;

    	public AskimetService(IDocumentSession session)
    	{
    		this.session = session;

    		akismetKey = session.Load<BlogConfig>("Blog/Config").AkismetKey;
    	}

    	public bool CheckForSpam(PostComments.Comment comment)
        {
            //Create a new instance of the Akismet API and verify your key is valid.
            string blog = ConfigurationManager.AppSettings["MainUrl"];
            var api = new Akismet(akismetKey, blog, comment.UserAgent);
            if (!api.VerifyKey()) throw new Exception("Akismet API key invalid.");

            var akismetComment = new AkismetComment
            {
                Blog = blog,
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

        public void MarkHam(PostComments.Comment comment)
        {
            //Create a new instance of the Akismet API and verify your key is valid.
            string blog = ConfigurationManager.AppSettings["MainUrl"];
			var api = new Akismet(akismetKey, blog, comment.UserAgent);
            if (!api.VerifyKey()) throw new Exception("Akismet API key invalid.");

            var akismetComment = new AkismetComment
            {
                Blog = blog,
                UserIp = comment.UserHostAddress,
                UserAgent = comment.UserAgent,
                CommentContent = comment.Body,
                CommentType = "comment",
                CommentAuthor = comment.Author,
                CommentAuthorEmail = comment.Email,
                CommentAuthorUrl = comment.Url,
            };

            #if Release
            api.SubmitHam(akismetComment);
            #endif
        }

        public void MarkSpam(PostComments.Comment comment)
        {
            //Create a new instance of the Akismet API and verify your key is valid.
            string blog = ConfigurationManager.AppSettings["MainUrl"];
			var api = new Akismet(akismetKey, blog, comment.UserAgent);
            if (!api.VerifyKey()) throw new Exception("Akismet API key invalid.");

            var akismetComment = new AkismetComment
            {
                Blog = blog,
                UserIp = comment.UserHostAddress,
                UserAgent = comment.UserAgent,
                CommentContent = comment.Body,
                CommentType = "comment",
                CommentAuthor = comment.Author,
                CommentAuthorEmail = comment.Email,
                CommentAuthorUrl = comment.Url,
            };

            #if Release
            api.SubmitSpam(akismetComment);
            #endif
        }
    }
}