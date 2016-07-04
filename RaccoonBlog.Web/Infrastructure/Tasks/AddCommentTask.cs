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

		private readonly CommentInput commentInput;
		private readonly RequestValues requestValues;
		private readonly int postId;

		public AddCommentTask(CommentInput commentInput, RequestValues requestValues, int postId)
		{
			this.commentInput = commentInput;
			this.requestValues = requestValues;
			this.postId = postId;
		}

		public override void Execute()
		{
			var post = DocumentSession
				.Include<Post>(x => x.AuthorId)
				.Include(x => x.CommentsId)
				.Load(postId);
			var postAuthor = DocumentSession.Load<User>(post.AuthorId);
			var comments = DocumentSession.Load<PostComments>(post.CommentsId);

			var comment = new PostComments.Comment
			              	{
			              		Id = comments.GenerateNewCommentId(),
			              		Author = commentInput.Name,
			              		Body = commentInput.Body,
			              		CreatedAt = DateTimeOffset.Now,
			              		Email = commentInput.Email,
			              		Url = commentInput.Url,
			              		Important = requestValues.IsAuthenticated, // TODO: Don't mark as important based on that
			              		UserAgent = requestValues.UserAgent,
			              		UserHostAddress = requestValues.UserHostAddress,
			              	};
			comment.IsSpam = AkismetService.CheckForSpam(comment);

			var commenter = DocumentSession.GetCommenter(commentInput.CommenterKey) ?? new Commenter { Key = commentInput.CommenterKey ?? Guid.Empty };
			SetCommenter(commenter, comment);

			if (requestValues.IsAuthenticated == false && comment.IsSpam)
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

			SendNewCommentEmail(post, comment, postAuthor);
		}

		private void SetCommenter(Commenter commenter, PostComments.Comment comment)
		{
			if (requestValues.IsAuthenticated)
				return;

			commentInput.MapPropertiesToInstance(commenter);
			commenter.IsTrustedCommenter = comment.IsSpam == false;

			if (comment.IsSpam)
				commenter.NumberOfSpamComments++;

			DocumentSession.Store(commenter);
			comment.CommenterId = commenter.Id;
		}

		private void SendNewCommentEmail(Post post, PostComments.Comment comment, User postAuthor)
		{
			if (requestValues.IsAuthenticated)
				return; // we don't send email for authenticated users

			var viewModel = comment.MapTo<NewCommentEmailViewModel>();
			viewModel.PostId = RavenIdResolver.Resolve(post.Id);
			viewModel.PostTitle = HttpUtility.HtmlDecode(post.Title);
			viewModel.PostSlug = SlugConverter.TitleToSlug(post.Title);
			viewModel.BlogName = DocumentSession.Load<BlogConfig>("Blog/Config").Title;
			viewModel.Key = post.ShowPostEvenIfPrivate.MapTo<string>();
		    viewModel.IsSpam = comment.IsSpam;

			var subject = string.Format("{2}Comment on: {0} from {1}", viewModel.PostTitle, viewModel.BlogName, viewModel.IsSpam ? "[DETECTED SPAM] " : string.Empty);

			TaskExecutor.ExcuteLater(new SendEmailTask(viewModel.Email, subject, "NewComment", postAuthor.Email, viewModel));
		}
	}
}