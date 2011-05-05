using System;
using System.Configuration;
using Joel.Net;
using RaccoonBlog.Web.Models;

namespace RaccoonBlog.Web.Services
{
    public class AskimetService
    {
        public bool CheckForSpam(PostComments.Comment comment)
        {
            //Create a new instance of the Akismet API and verify your key is valid.
            string blog = ConfigurationManager.AppSettings["MainUrl"];
            var api = new Akismet(ConfigurationManager.AppSettings["AkismetKey"], blog, comment.UserAgent);
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
            var api = new Akismet(ConfigurationManager.AppSettings["AkismetKey"], blog, comment.UserAgent);
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
            var api = new Akismet(ConfigurationManager.AppSettings["AkismetKey"], blog, comment.UserAgent);
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