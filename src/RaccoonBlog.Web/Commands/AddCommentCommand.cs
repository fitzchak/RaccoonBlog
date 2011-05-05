using System;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;
using RaccoonBlog.Web.Infrastructure.Commands;
using RaccoonBlog.Web.Mailers;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.Services;
using RaccoonBlog.Web.ViewModels;
using Raven.Client;

namespace RaccoonBlog.Web.Commands
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
            if (Session == null)
                throw new NullReferenceException();

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
                Url = _commentInput.Url,
                Important = _requestValues.IsAuthenticated,
                UserAgent = _requestValues.UserAgent,
                UserHostAddress = _requestValues.UserHostAddress
            };
            comment.IsSpam = new AskimetService().CheckForSpam(comment);

        	if (comment.IsSpam)
        		comments.Spam.Add(comment);
        	else
        		comments.Comments.Add(comment);

            var viewModel = comment.MapTo<NewCommentEmailViewModel>();
            viewModel.PostId = RavenIdResolver.Resolve(post.Id);
            viewModel.PostTitle = post.Title;
            var message = new MailTemplates().NewComment(viewModel);
            CommandExcucator.ExcuteLater(new SendEmailCommand(message));
        }

        public void Execute(IDocumentSession session)
        {
            Session = session;
            Execute();
        }
    }
}