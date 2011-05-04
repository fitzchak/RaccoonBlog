using System;
using System.Configuration;
using Joel.Net;
using RavenDbBlog.Core.Models;
using RavenDbBlog.ViewModels;

namespace RavenDbBlog.Services
{
    public class AskimetService
    {
        private readonly RequestValues _requestValues;

        public AskimetService(RequestValues requestValues)
        {
            _requestValues = requestValues;
        }

        public bool CheckForSpam(CommentInput commentInput)
        {
            //Create a new instance of the Akismet API and verify your key is valid.
            string blog = "http://" + ConfigurationManager.AppSettings["MainUrl"];
            var api = new Akismet(ConfigurationManager.AppSettings["AkismetKey"], blog, _requestValues.UserAgent);
            if (!api.VerifyKey()) throw new Exception("Akismet API key invalid.");

            var akismetComment = new AkismetComment
            {
                Blog = blog,
                UserIp = _requestValues.UserHostAddress,
                UserAgent = _requestValues.UserAgent,
                CommentContent = commentInput.Body,
                CommentType = "comment",
                CommentAuthor = commentInput.Name,
                CommentAuthorEmail = commentInput.Email,
                CommentAuthorUrl = commentInput.Url,
            };

            //Check if Akismet thinks this comment is spam. Returns TRUE if spam.
            return api.CommentCheck(akismetComment);
        }

        public void MarkHum(PostComments.Comment comment)
        {
            //Create a new instance of the Akismet API and verify your key is valid.
            string blog = "http://" + ConfigurationManager.AppSettings["MainUrl"];
            var api = new Akismet(ConfigurationManager.AppSettings["AkismetKey"], blog, _requestValues.UserAgent);
            if (!api.VerifyKey()) throw new Exception("Akismet API key invalid.");

            var akismetComment = new AkismetComment
            {
                Blog = blog,
                UserIp = _requestValues.UserHostAddress,
                UserAgent = _requestValues.UserAgent,
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
            string blog = "http://" + ConfigurationManager.AppSettings["MainUrl"];
            var api = new Akismet(ConfigurationManager.AppSettings["AkismetKey"], blog, _requestValues.UserAgent);
            if (!api.VerifyKey()) throw new Exception("Akismet API key invalid.");

            var akismetComment = new AkismetComment
            {
                Blog = blog,
                UserIp = _requestValues.UserHostAddress,
                UserAgent = _requestValues.UserAgent,
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