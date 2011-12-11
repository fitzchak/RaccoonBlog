using System;
using System.Web;
using HibernatingRhinos.Loci.Common.Tasks;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;
using RaccoonBlog.Web.Infrastructure.Common;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.Services;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Infrastructure.Tasks
{
	public class AddCommentTask : BackgroundTask
	{
		public class RequestValues
		{
			public string UserAgent { get; set; }
			public string UserHostAddress { get; set; }
			public bool IsAuthenticated { get; set; }
		}

		private readonly CommentInput _commentInput;
		private readonly RequestValues _requestValues;
		private readonly int _postId;

		public AddCommentTask(CommentInput commentInput, RequestValues requestValues, int postId)
		{
			_commentInput = commentInput;
			_requestValues = requestValues;
			_postId = postId;
		}

		public override void Execute()
		{
			var post = DocumentSession.Include<Post>(x => x.AuthorId).Load(_postId);
			var postAuthor = DocumentSession.Load<User>(post.AuthorId);
			var comments = DocumentSession.Load<PostComments>(_postId);

			var comment = new PostComments.Comment
			              	{
			              		Id = comments.GenerateNewCommentId(),
			              		Author = _commentInput.Name,
			              		Body = _commentInput.Body,
			              		CreatedAt = DateTimeOffset.Now,
			              		Email = _commentInput.Email,
			              		Url = _commentInput.Url,
			              		Important = _requestValues.IsAuthenticated, // TODO: Don't mark as important based on that
			              		UserAgent = _requestValues.UserAgent,
			              		UserHostAddress = _requestValues.UserHostAddress
			              	};
			comment.IsSpam = AkismetService.CheckForSpam(comment);

			var commenter = DocumentSession.GetCommenter(_commentInput.CommenterKey) ?? new Commenter { Key = _commentInput.CommenterKey ?? Guid.Empty };

			if (_requestValues.IsAuthenticated == false && comment.IsSpam)
			{
				if (commenter.NumberOfSpamComments > 4)
					return;
				comments.Spam.Add(comment);
			}
			else
			{
				post.CommentsCount++;
				comments.Comments.Add(comment);
			}

			SetCommenter(commenter, comment.IsSpam);

			SendNewCommentEmail(post, comment, postAuthor);
		}

		private void SetCommenter(Commenter commenter, bool isSpamComment)
		{
			if (_requestValues.IsAuthenticated)
				return;

			_commentInput.MapPropertiesToInstance(commenter);
			commenter.IsTrustedCommenter = isSpamComment == false;

			if (isSpamComment)
				commenter.NumberOfSpamComments++;

			DocumentSession.Store(commenter);
		}

		private void SendNewCommentEmail(Post post, PostComments.Comment comment, User postAuthor)
		{
			if (_requestValues.IsAuthenticated)
				return; // we don't send email for authenticated users

			var viewModel = comment.MapTo<NewCommentEmailViewModel>();
			viewModel.PostId = RavenIdResolver.Resolve(post.Id);
			viewModel.PostTitle = HttpUtility.HtmlDecode(post.Title);
			viewModel.PostSlug = SlugConverter.TitleToSlug(post.Title);
			viewModel.BlogName = DocumentSession.Load<BlogConfig>("Blog/Config").Title;
			viewModel.Key = post.ShowPostEvenIfPrivate.MapTo<string>();

			var subject = string.Format("{2}Comment on: {0} from {1}", viewModel.PostTitle, viewModel.BlogName, comment.IsSpam ? "[Spam] " : string.Empty);

			TaskExecutor.ExcuteLater(new SendEmailTask(viewModel.Email, subject, "NewComment", postAuthor.Email, viewModel));
		}
	}
}
