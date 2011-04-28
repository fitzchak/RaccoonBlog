using System;
using System.Web.Mvc;
using Raven.Client;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Infrastructure.Commands;
using RavenDbBlog.Services;
using RavenDbBlog.ViewModels;
using RazorEngine;

namespace RavenDbBlog.Commands
{
    public class AddCommentCommand : ICommand
    {
        private readonly CommentInput _commentInput;
        private readonly RequestValues _requestValues;
        private readonly int _postId;

        public IDocumentSession Session { get; set; }

        public AddCommentCommand(CommentInput commentInput, RequestValues requestValues, int postId)
        {
            _commentInput = commentInput;
            _requestValues = requestValues;
            _postId = postId;
        }

        public void Execute()
        {
            var post = Session.Load<Post>(_postId);
            var comments = Session.Load<PostComments>(_postId);

            post.CommentsCount++;

            var comment = new PostComments.Comment
            {
                Id = comments.GenerateNewCommentId(),
                Author = _commentInput.Name,
                Body = _commentInput.Body,
                CreatedAt = DateTimeOffset.Now,  
                Email = _commentInput.Email,
                Important = _requestValues.IsAuthenticated,
                Url = _commentInput.Url,
                IsSpam = new AskimetService(_requestValues).CheckForSpam(_commentInput),
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
    }
}