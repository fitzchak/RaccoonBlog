using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HibernatingRhinos.Loci.Common.Tasks;
using RaccoonBlog.Web.Helpers;
using RaccoonBlog.Web.Helpers.Validation;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;
using RaccoonBlog.Web.Infrastructure.Common;
using RaccoonBlog.Web.Infrastructure.Indexes;
using RaccoonBlog.Web.Infrastructure.Tasks;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Controllers
{
    public partial class PostDetailsController : RaccoonController
    {
        public virtual ActionResult Details(int id, string slug, Guid key)
        {
            var post = RavenSession
                .Include<Post>(x => x.CommentsId)
                .Include(x => x.AuthorId)
                .Load("posts/" + id);

            if (post == null)
                return HttpNotFound();

            if (post.IsPublicPost(key) == false)
                return HttpNotFound();

            SeriesInfo seriesInfo = GetSeriesInfo(post.Title);

            var comments = RavenSession.Load<PostComments>(post.CommentsId) ?? new PostComments();
            var vm = new PostViewModel
            {
                Post = post.MapTo<PostViewModel.PostDetails>(),
                Comments = comments.Comments
                            .OrderBy(x => x.CreatedAt)
                            .MapTo<PostViewModel.Comment>(),
                NextPost = RavenSession.GetNextPrevPost(post, true),
                PreviousPost = RavenSession.GetNextPrevPost(post, false),
                AreCommentsClosed = comments.AreCommentsClosed(post, BlogConfig.NumberOfDayToCloseComments),
                SeriesInfo = seriesInfo
            };

            vm.Post.Author = RavenSession.Load<User>(post.AuthorId).MapTo<PostViewModel.UserDetails>();

            var comment = TempData["new-comment"] as CommentInput;

            if (comment != null)
            {
                var newCommentEmailHash = EmailHashResolver.Resolve(comment.Email);
                var newCommentContent = MarkdownResolver.Resolve(comment.Body);
                if (vm.Comments.Any(x =>
                    x.Author == comment.Name
                    && x.EmailHash == newCommentEmailHash
                    && x.Body.ToString() == newCommentContent.ToString()) == false)
                {
                    vm.Comments.Add(new PostViewModel.Comment
                    {
                        CreatedAt = DateTimeOffset.Now.UtcDateTime.ToString(),
                        Author = comment.Name,
                        Body = newCommentContent,
                        Id = -1,
                        Url = UrlResolver.Resolve(comment.Url),
                        Tooltip = "Comment by " + comment.Name,
                        EmailHash = newCommentEmailHash
                    });
                }
            }

            if (vm.Post.Slug != slug)
                return RedirectToActionPermanent("Details", new { id, vm.Post.Slug });

            SetWhateverUserIsTrustedCommenter(vm);

            return View("Details", vm);
        }

        [ValidateInput(false)]
        [HttpPost]
        public virtual ActionResult Comment(CommentInput input, int id, Guid key)
        {
            if (ModelState.IsValid)
            {
                var post = RavenSession
                    .Include<Post>(x => x.CommentsId)
                    .Load("posts/" + id);

                if (post == null || post.IsPublicPost(key) == false)
                    return HttpNotFound();

                var comments = RavenSession.Load<PostComments>(post.CommentsId);
                if (comments == null)
                    return HttpNotFound();

                var commenter = RavenSession.GetCommenter(input.CommenterKey);
                if (commenter == null)
                {
                    input.CommenterKey = Guid.NewGuid();
                }

                ValidateCommentsAllowed(post, comments);
                ValidateCaptcha(input, commenter);

                if (ModelState.IsValid == false)
                    return PostingCommentFailed(post, input, key);

                TaskExecutor.ExcuteLater(new AddCommentTask(input, Request.MapTo<AddCommentTask.RequestValues>(), id));

                CommenterUtil.SetCommenterCookie(Response, input.CommenterKey.MapTo<string>());

                OutputCacheManager.RemoveItem(SectionController.NameConst, MVC.Section.ActionNames.List);

                return PostingCommentSucceeded(post, input);
            }

            return RedirectToAction("Details");
        }

        private bool CheckRecaptchaChallengeSupplied()
        {
            var recaptchaChallengeField = Request.Form.GetValues("recaptcha_challenge_field");
            if (recaptchaChallengeField == null
                || recaptchaChallengeField.Length == 0
                || recaptchaChallengeField.GetValue(0) as string == string.Empty)
            {
                return false;
            }

            return true;
        }

        private ActionResult PostingCommentSucceeded(Post post, CommentInput input)
        {
            const string successMessage = "Your comment will be posted soon. Thanks!";
            if (Request.IsAjaxRequest())
                return Json(new { Success = true, message = successMessage });

            TempData["new-comment"] = input;
            var postReference = post.MapTo<PostReference>();

            return Redirect(Url.Action("Details",
                new { Id = postReference.DomainId, postReference.Slug, key = post.ShowPostEvenIfPrivate }) + "#comments-form-location");
        }

        private void ValidateCommentsAllowed(Post post, PostComments comments)
        {
            if (comments.AreCommentsClosed(post, BlogConfig.NumberOfDayToCloseComments))
                ModelState.AddModelError("CommentsClosed", "This post is closed for new comments.");
            if (post.AllowComments == false)
                ModelState.AddModelError("CommentsClosed", "This post does not allow comments.");
        }

        private void ValidateCaptcha(CommentInput input, Commenter commenter)
        {
            if (Request.IsAuthenticated ||
                (commenter != null && commenter.IsTrustedCommenter == true))
                return;

            var captchaChallegeSupplied = CheckRecaptchaChallengeSupplied();
            if (captchaChallegeSupplied == false)
            {
                ModelState.AddModelError("CaptchaNotValid", "ReCaptcha challenge was not supplied.");
                return;
            }

            if (RecaptchaValidatorWrapper.Validate(ControllerContext.HttpContext))
                return;

            ModelState.AddModelError("CaptchaNotValid",
                                     "You did not type the verification word correctly. Please try again.");
        }

        private ActionResult PostingCommentFailed(Post post, CommentInput input, Guid key)
        {
            if (Request.IsAjaxRequest())
                return Json(new { Success = false, message = ModelState.FirstErrorMessage() });

            var postReference = post.MapTo<PostReference>();
            var result = Details(postReference.DomainId, postReference.Slug, key);
            var model = result as ViewResult;
            if (model != null)
            {
                var viewModel = model.Model as PostViewModel;
                if (viewModel != null)
                    viewModel.Input = input;
            }
            return result;
        }

        private void SetWhateverUserIsTrustedCommenter(PostViewModel vm)
        {
            if (Request.IsAuthenticated)
            {
                var user = RavenSession.GetCurrentUser();
                vm.Input = user.MapTo<CommentInput>();
                vm.IsTrustedCommenter = true;
                vm.IsLoggedInCommenter = true;
                return;
            }

            var cookie = Request.Cookies[CommenterUtil.CommenterCookieName];
            if (cookie == null) return;

            var commenter = RavenSession.GetCommenter(cookie.Value);
            if (commenter == null)
            {
                vm.IsLoggedInCommenter = false;
                Response.Cookies.Set(new HttpCookie(CommenterUtil.CommenterCookieName) { Expires = DateTime.Now.AddYears(-1) });
                return;
            }

            vm.IsLoggedInCommenter = string.IsNullOrWhiteSpace(commenter.OpenId) == false;
            vm.Input = commenter.MapTo<CommentInput>();
            vm.IsTrustedCommenter = commenter.IsTrustedCommenter == true;
        }

        private SeriesInfo GetSeriesInfo(string title)
        {
            SeriesInfo seriesInfo = null;
            string seriesTitle = TitleConverter.ToSeriesTitle(title);

            if (!string.IsNullOrEmpty(seriesTitle))
            {
                var series = RavenSession.Query<Posts_Series.Result, Posts_Series>()
                    .Where(x => x.Series.StartsWith(seriesTitle) && x.Count > 1)
                    .OrderByDescending(x => x.MaxDate)
                    .FirstOrDefault();

                if (series == null)
                    return null;

                var postsInSeries = GetPostsForCurrentSeries(series);

                seriesInfo = new SeriesInfo
                {
                    SeriesId = series.SeriesId,
                    SeriesTitle = seriesTitle,
                    PostsInSeries = postsInSeries
                };
            }

            return seriesInfo;
        }

        private IList<PostInSeries> GetPostsForCurrentSeries(Posts_Series.Result series)
        {
            IList<PostInSeries> postsInSeries = null;

            if (series != null)
            {
                postsInSeries = series
                    .Posts
                    .Select(s => new PostInSeries
                    {
                        Id = RavenIdResolver.Resolve(s.Id),
                        Slug = SlugConverter.TitleToSlug(s.Title),
                        Title = HttpUtility.HtmlDecode(TitleConverter.ToPostTitle(s.Title)),
                        PublishAt = s.PublishAt
                    })
                    .OrderByDescending(p => p.PublishAt)
                    .ToList();
            }

            return postsInSeries;
        }
    }
}