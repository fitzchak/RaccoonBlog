using System;
using System.Diagnostics;
using Raven.Client.Document;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Infrastructure.EntityFramework;
using System.Linq;
using RavenPost = RavenDbBlog.Core.Models.Post;

namespace RavenDbBlog.Import
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var e = new SubtextEntities())
            {
            	Console.WriteLine("Starting...");

                var sp = Stopwatch.StartNew();
                var theEntireDatabaseOhMygod = e.Posts
                    .Include("Comments")
                    .Include("Links")
                    .Include("Links.Categories")
                    .ToList()
					.OrderBy(x=>x.DateSyndicated);

                Console.WriteLine("Loading data took {0:#,#} ms", sp.ElapsedMilliseconds);



                using (var store = new DocumentStore
                                       {
                                           Url = "http://localhost:8080",
                                       }.Initialize())
                {
					using(var s = store.OpenSession())
					{
					    var users = new[]
					        {
					            new {Email = "ayende@ayende.com", FullName = "Ayende Rahien"},
					            new {Email = "fitzchak@ayende.com", FullName = "Fitzchak Yitzchaki"},
					        };

					    for (int i = 0; i < users.Length; i++)
					    {
					        var user = new User
					            {
                                    Id = "users/" + (i + 1),
                                    Email = users[i].Email,
                                    Enabled = true,
					            };
                            user.SetPassword("123456");
                            s.Store(user);
					    }
						s.SaveChanges();
					}

                    foreach (var post in theEntireDatabaseOhMygod)
                    {
                        var ravenPost = new RavenPost
                        {
                            Author = post.Author,
                            CreatedAt = new DateTimeOffset(post.DateAdded),
                            PublishAt = new DateTimeOffset(post.DateSyndicated ?? post.DateAdded),
                            Body = post.Text,
                            CommentsCount = post.FeedBackCount,
                            LegacySlug = post.EntryName,
                            Title = post.Title,
                            Tags = post.Links.Select(x=>x.Categories.Title)
                                .Where(x => x != "Uncategorized")
                                .ToArray()
                        };

                        var commentsCollection = new PostComments
                                               {
                                                   Comments = post.Comments
                                                       .Where(comment => comment.StatusFlag != 5)
                                                       .Select(
                                                           comment => new PostComments.Comment
                                                                          {
                                                                              Author = comment.Author,
                                                                              Body = comment.Body,
                                                                              CreatedAt = comment.DateCreated,
                                                                              Email = comment.Email,
                                                                              Important = comment.IsBlogAuthor ?? false,
                                                                              Url = comment.Url
                                                                          }
                                                       ).ToList(),
                                                   Spam = post.Comments
                                                       .Where(comment => comment.StatusFlag == 12)
                                                       .Select(
                                                           comment => new PostComments.Comment
                                                                          {
                                                                              Author = comment.Author,
                                                                              Body = comment.Body,
                                                                              CreatedAt = comment.DateCreated,
                                                                              Email = comment.Email,
                                                                              Important = comment.IsBlogAuthor ?? false,
                                                                              Url = comment.Url
                                                                          }
                                                       ).ToList(),
                                               };

                        using (var session = store.OpenSession())
                        {
                            session.Store(commentsCollection);
                            ravenPost.CommentsId = commentsCollection.Id;

                            session.Store(ravenPost);

                            session.SaveChanges();
                        }

                    }
                }

                Console.WriteLine(sp.Elapsed);
            }

        }
    }
}
