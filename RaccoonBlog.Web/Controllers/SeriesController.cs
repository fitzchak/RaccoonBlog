namespace RaccoonBlog.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using RaccoonBlog.Web.Infrastructure.AutoMapper;
    using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;
    using RaccoonBlog.Web.Infrastructure.Common;
    using RaccoonBlog.Web.Infrastructure.Indexes;
    using RaccoonBlog.Web.ViewModels;
    using Raven.Client.Linq;

    public class SeriesController : RaccoonController
    {
        public ActionResult PostsSeries()
        {
            var series = RavenSession.Query<Posts_Series.Result, Posts_Series>()
                .Where(x => x.Count > 1)
                .OrderByDescending(x => x.MaxDate)
                .ToList();

            var vm = new List<SeriesPostsViewModel>();

            foreach (var result in series)
            {
                var svm = result.MapTo<SeriesPostsViewModel>();
                foreach (var post in result.Posts)
                {
                    svm.PostsInSeries.Add(post.MapTo<PostInSeries>());
                }
                vm.Add(svm);
            }

            return View(vm);
        }
    }
}