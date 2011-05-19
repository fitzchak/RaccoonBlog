using System.Linq;
using RaccoonBlog.Web.Infrastructure;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
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
            return RedirectToActionPermanent("Details", "PostDetails", new { Id = postReference.DomainId, postReference.Slug });
        }

        public ActionResult RedirectLegacyArchive(int year, int month, int day)
        {
            return RedirectToActionPermanent("Archive", "Post", new { year, month, day });
        }
    }
}
