using System;
using System.Linq;
using Raven.Client.Indexes;
using RavenDbBlog.Core.Models;

namespace RavenDbBlog.Indexes
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

    public class TagCount
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public DateTimeOffset LastSeenAt { get; set; }
    }

    public class TempTagCount
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
}