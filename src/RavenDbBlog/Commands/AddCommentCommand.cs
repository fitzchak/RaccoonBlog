using System;
using System.Configuration;
using System.Web.Mvc;
using Joel.Net;
using Raven.Client;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Infrastructure.Commands;
using RavenDbBlog.ViewModels;
using RazorEngine;

namespace RavenDbBlog.Commands
{
    public class AddCommentCommand : ICommand
    {
        public CommentInput CommentInput { get; set; }
        public int PostId { get; set; }

        public IDocumentSession Session { get; set; }

        public void Execute()
        {
            Session.Load<object>("posts/" + PostId, "posts/" + PostId + "/comments");

            var post = Session.Load<Post>("posts/" + PostId);
            var comments = Session.Load<CommentsCollection>("posts/" + PostId + "/comments");

            post.CommentsCount++;

            var comment = new CommentsCollection.Comment
            {
                Author = CommentInput.Name,
                Body = CommentInput.Body,
                CreatedAt = DateTimeOffset.Now,  // TODO: Time zone of the client.
                Email = CommentInput.Email,
                Important = false, //TODO: How to figure this out?
                Url = CommentInput.Url,
                IsSpam = CheckForSpam(),
            };

            if (comment.IsSpam)
            {
                comments.Spam.Add(comment);
            }
            else
            {
                comments.Comments.Add(comment);
            }

            var vm = new NewCommentEmailViewModel
            {
                Author = comment.Author,
                Body = MvcHtmlString.Create(comment.Body),
                CreatedAt = comment.CreatedAt,
                Email = comment.Email,
                Url = comment.Url,
            };

            var emailContents = Razor.Run(vm, "NewComment");

            CommandExcucator.ExcuteLater(new SendEmailCommand()
                                             {
                                                 Subject = "Comment: " + post.Title,
                                                 Contents = emailContents
                                             });
        }


        private bool CheckForSpam()
        {
            //Create a new instance of the Akismet API and verify your key is valid.
            string blog = "http://" + ConfigurationSettings.AppSettings["MainUrl"];
            var api = new Akismet(ConfigurationSettings.AppSettings["AkismetKey"], blog, CommentInput.Request.UserAgent);
            if (!api.VerifyKey()) throw new Exception("Akismet API key invalid.");

            var akismetComment = new AkismetComment
            {
                Blog = blog,
                UserIp = CommentInput.Request.UserHostAddress,
                UserAgent = CommentInput.Request.UserAgent,
                CommentContent = CommentInput.Body,
                CommentType = "comment",
                CommentAuthor = CommentInput.Name,
                CommentAuthorEmail = CommentInput.Email,
                CommentAuthorUrl = CommentInput.Url,
            };

            //Check if Akismet thinks this comment is spam. Returns TRUE if spam.
            return api.CommentCheck(akismetComment);
        }
    }
}