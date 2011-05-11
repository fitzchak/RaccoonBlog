using System;
using System.Linq;
using RaccoonBlog.Web.Models;
using Raven.Client.Indexes;

namespace RaccoonBlog.Web.Infrastructure.Indexes
{
    public class Tags_Count : AbstractIndexCreationTask<Post, TagCount>
    {
        public Tags_Count()
        {
            Map = posts => from post in posts
                           from tag in post.Tags
                           select new {Name = tag, Count = 1, LastSeenAt = post.PublishAt};
            Reduce = results => from tagCount in results
                                group tagCount by tagCount.Name
                                into g
                                select new {Name = g.Key, Count = g.Sum(x => x.Count), LastSeenAt = g.Max(x=>(DateTimeOffset)x.LastSeenAt) };
        }
    }
}
