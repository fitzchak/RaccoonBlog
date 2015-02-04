using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RaccoonBlog.Web.Infrastructure.Indexes
{
    using RaccoonBlog.Web.Models;
    using Raven.Client.Indexes;

    public class Posts_Series : AbstractIndexCreationTask<Post, Posts_Series.Result>
    {
        public class Result
        {
            public Result()
            {
                Posts = new List<PostInformation>();
            }
            public string Series { get; set; }
            public List<PostInformation> Posts { get; set; }
            public int Count { get; set; }
            public DateTimeOffset MaxDate { get; set; }
            public DateTimeOffset MinDate { get; set; }
            public int Days { get; set; }
        }

        public class PostInformation
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public DateTimeOffset PublishAt { get; set; }
        }

        public Posts_Series()
        {
            Map = posts => from p in posts
                           let parts = p.Title.Split(':')
                           where parts.Length > 1
                           let series = parts[0].Trim().ToLower()
                           let date = p.PublishAt == null ? DateTimeOffset.MinValue : p.PublishAt
                           select new
                           {
                               Series = series,
                               Posts = new[] { new { p.Id, p.Title, p.PublishAt } },
                               Count = 1,
                               MaxDate = date,
                               MinDate = date,
                               Days = 1
                           };

            Reduce = results => from r in results
                         group r by r.Series
                         into g
                         let maxDate = g.Select(x => x.MaxDate).Max()
                         let minDate = g.OrderBy(x => x.MinDate).FirstOrDefault().MinDate
                         select new
                         {
                             Series = g.Key,
                             Posts = g.SelectMany(x => x.Posts),
                             Count = g.Sum((x => x.Count)),
                             MaxDate = maxDate,
                             MinDate = minDate,
                             Days = (int)(maxDate - minDate).TotalDays
                         };
        }
    }
}