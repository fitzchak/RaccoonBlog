using System;
using System.Diagnostics;
using Raven.Client.Document;
using RavenDbBlog.Infrastructure.EntityFramework;
using System.Linq;
using RavenPost = RavenDbBlog.Core.Models.Post;
using RavenComment = RavenDbBlog.Core.Models.CommentsCollection;
namespace RavenDbBlog.Import
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var e = new SubtextEntities())
            {
                var sp = Stopwatch.StartNew();
                var theEntireDatabaseOhMygod = e.Posts
                    .Include("Comments")
                    .Include("Links")
                    .Include("Links.Categories")
                    .ToList();

                Console.WriteLine("Loading data took {0:#,#} ms", sp.ElapsedMilliseconds);



                using (var store = new DocumentStore
                                       {
                                           Url = "http://localhost:8080",
                                       }.Initialize())
                {
                    foreach (var post in theEntireDatabaseOhMygod)
                    {
                        var ravenPost = new RavenPost
                        {
                            Author = post.Author,
                            CreatedAt = new DateTimeOffset(post.DateAdded),
                            PublishAt = new DateTimeOffset(post.DateSyndicated ?? post.DateAdded),
                            Body = post.Text,
                            CommentsCount = post.FeedBackCount,
                            Id = "posts/" + post.ID,
                            Slug = post.EntryName,
                            Title = post.Title,
                            Tags = post.Links.Select(x=>x.Categories.Title)
                                .Where(x => x != "Uncategorized")
                                .ToList()
                        };

                        var ravenComment = new RavenComment
                        {
                            PostId = "posts/" + post.ID,
                            Id = "posts/" + post.ID + "/comments",
                            Comments = post.Comments.Select(
                                comment => new RavenComment.Comment
                                {
                                    Author = comment.Author,
                                    Body = comment.Body,
                                    CreatedAt = comment.DateCreated,
                                    Email = comment.Email,
                                    Important = comment.IsBlogAuthor ?? false,
                                    Url = comment.Url
                                }
                                ).ToList()
                        };

                        using (var session = store.OpenSession())
                        {
                            session.Store(ravenPost);
                            session.Store(ravenComment);
                            session.SaveChanges();
                        }

                    }
                }

                Console.WriteLine(sp.Elapsed);
            }

        }
    }
}
