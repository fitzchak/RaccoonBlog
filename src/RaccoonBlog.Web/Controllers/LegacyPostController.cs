using System;
using System.Linq;
using RaccoonBlog.Web.Common;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.Indexes;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
using Raven.Client.Linq;
using System.Web.Mvc;

namespace RaccoonBlog.Web.Controllers
{
    public class LegacyPostController : AbstractController
    {
        public ActionResult RedirectLegacyPost(int year, int month, int day, string slug)
        {
            var postQuery = Session.Query<Post>()
                .WhereIsPublicPost()
                .Where(post1 => post1.LegacySlug == slug &&
                        (post1.PublishAt.Year == year && post1.PublishAt.Month == month && post1.PublishAt.Day == day));

            var post = postQuery.FirstOrDefault();
            if (post == null)
                return HttpNotFound();

            var postReference = post.MapTo<PostReference>();
            return RedirectToActionPermanent("Details", new { Id = postReference.DomainId, postReference.Slug });
        }
    }
}