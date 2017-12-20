using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using RaccoonBlog.Web.Areas.Admin.Models;
using RaccoonBlog.Web.Areas.Admin.ViewModels;
using RaccoonBlog.Web.Helpers;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.Models.SocialNetwork;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;

namespace RaccoonBlog.Web.Areas.Admin.Controllers
{
    public partial class SettingsController : AdminController
    {
        [HttpGet]
        public virtual ActionResult Index()
        {
            return View(BlogConfig);
        }

        [HttpPost]
        public virtual ActionResult Index(BlogConfig config)
        {
            if (ModelState.IsValid == false)
            {
                ViewBag.Message = ModelState.FirstErrorMessage();
                if (Request.IsAjaxRequest())
                    return Json(new { Success = false, ViewBag.Message });
                return View(BlogConfig);
            }

            var current = RavenSession.Load<BlogConfig>(BlogConfig.Key);
            if (IsFuturePostsEncryptionOptionsChanged(current, config))
            {
                RemoveFutureRssAccessOnEncryptionConfigChange();
            }

            RavenSession.Advanced.Evict(current);
            RavenSession.Store(config, BlogConfig.Key);
            RavenSession.SaveChanges();

            OutputCacheManager.RemoveItem(MVC.Section.Name, MVC.Section.ActionNames.ContactMe);

            ViewBag.Message = "Configurations successfully saved!";
            if (Request.IsAjaxRequest())
                return Json(new { Success = true, ViewBag.Message });
            return View(config);
        }

        private void RemoveFutureRssAccessOnEncryptionConfigChange()
        {
            var store = RavenSession.Advanced.DocumentStore;
            var collectionName = store.Conventions.GetCollectionName(typeof(FutureRssAccess));
#pragma warning disable 4014
            store.Operations.SendAsync(new DeleteByQueryOperation(new IndexQuery { Query = "FROM " + collectionName }, new QueryOperationOptions
#pragma warning restore 4014
            {
                AllowStale = false
            }));
        }

        [HttpGet]
        public virtual async Task<ActionResult> RedditSubmission()
        {
            var model = await PrepareRedditManualSubmissionViewModel();
            return View(model);
        }

        [HttpGet]
        public virtual async Task<ActionResult> SubmitToReddit(string postId, string sr)
        {
            var post = RavenSession.Load<Post>(postId);
            var redditSubmitUrl = RedditHelper.SubmitUrl(sr, post);
            var postSubmission = post.Integration.Reddit.GetPostSubmissionForSubreddit(sr);
            postSubmission.Status = Reddit.SubmissionStatus.ManualSubmissionPending;
            postSubmission.Attempts = 0;

            RavenSession.SaveChanges();

            return Redirect(redditSubmitUrl);
        }

        [HttpGet]
        public virtual async Task<ActionResult> ResetFailedRedditSubmission(string postId, string sr)
        {
            var post = RavenSession.Load<Post>(postId);
            var postSubmission = post.Integration.Reddit.GetPostSubmissionForSubreddit(sr);
            postSubmission.Status = null;
            postSubmission.Attempts = 0;
            RavenSession.SaveChanges();
            return RedirectToAction(MVC.Admin.Settings.ActionNames.RedditSubmission);
        }

        private async Task<RedditManualSubmissionViewModel> PrepareRedditManualSubmissionViewModel()
        {
            var model = new RedditManualSubmissionViewModel();
            model.SubredditsToSubmitTo = RedditHelper.ParseSubreddits(BlogConfig);
            model.NotSubmittedPosts = RedditHelper.GetPostsForManualRedditSubmission(RavenSession, DateTimeOffset.UtcNow);
            return model;
        }

        [HttpGet]
        public virtual ActionResult RssFutureAccess()
        {
            ValidateConfiguration();
            SetFutureRssAccessList(RavenSession);

            return View(FutureRssAccess.Default);
        }

        [HttpPost]
        public virtual ActionResult RssFutureAccess(FutureRssAccess input)
        {
            ValidateConfiguration();

            if (ExistsRssFutureAccess(input))
            {
                ModelState.AddModelError("Exists", "Access has already been set up with same parameters.");
            }

            if (ModelState.IsValid == false)
            {
                SetFutureRssAccessList(RavenSession);
                return View(input);
            }

            input.Token = GetFutureAccessToken(input.ExpiredOn, input.NumberOfFutureDays, input.User);

            RavenSession.Store(input);
            RavenSession.SaveChanges();

            SetFutureRssAccessList(RavenSession);

            return View(input);
        }

        private bool ExistsRssFutureAccess(FutureRssAccess input)
        {
            var exists = RavenSession.Query<FutureRssAccess>()
                .Any(x => x.ExpiredOn == input.ExpiredOn &&
                          x.NumberOfFutureDays == input.NumberOfFutureDays &&
                          x.User == input.User);
            return exists;
        }

        private bool IsFuturePostsEncryptionOptionsChanged(BlogConfig current, BlogConfig config)
        {
            return current.FuturePostsEncryptionKey != config.FuturePostsEncryptionKey ||
                   current.FuturePostsEncryptionIv != config.FuturePostsEncryptionIv ||
                   current.FuturePostsEncryptionSalt != config.FuturePostsEncryptionSalt;
        }

        private void SetFutureRssAccessList(IDocumentSession session)
        {
            ViewData["FutureRssAccessList"] = session.Query<FutureRssAccess>()
                .Customize(x => x.WaitForNonStaleResults())
                .OrderBy(x => x.ExpiredOn)
                .ToList();
        }

        private void ValidateConfiguration()
        {
            if (string.IsNullOrEmpty(BlogConfig.FuturePostsEncryptionKey))
            {
                ModelState.AddModelError("BlogConfig.FuturePostsEncryptionKey", $"Future posts RSS token encryption key is not set.");
            }

            if (string.IsNullOrEmpty(BlogConfig.FuturePostsEncryptionIv))
            {
                ModelState.AddModelError("BlogConfig.FuturePostsEncryptionIv", $"Future posts RSS token encryption IV is not set.");
            }

            if (string.IsNullOrEmpty(BlogConfig.FuturePostsEncryptionSalt))
            {
                ModelState.AddModelError("BlogConfig.FuturePostsEncryptionSalt", $"Future posts RSS token encryption Salt is missing.");
            }
        }

        private string GetFutureAccessToken(DateTime expiresOn, int numberOfDays, string user)
        {
            return new FutureRssAccessToken(expiresOn, numberOfDays, user)
                .GetToken(BlogConfig);
        }
    }
}