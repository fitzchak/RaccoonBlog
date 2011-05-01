using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Xsl;
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
            //GetAllHtmlFromComments();
            //return;

            using (var e = new SubtextEntities())
            {
            	Console.WriteLine("Starting...");

                var sp = Stopwatch.StartNew();
                var theEntireDatabaseOhMygod = e.Posts
                    .Include("Comments")
                    .Include("Links")
                    .Include("Links.Categories")
                    .ToList()
                    .OrderBy(x => x.DateSyndicated);

                Console.WriteLine("Loading data took {0:#,#} ms", sp.ElapsedMilliseconds);



                using (var store = new DocumentStore
                                       {
                                           Url = "http://localhost:8080",
                                       }.Initialize())
                {
                    using (var s = store.OpenSession())
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
                                    FullName = users[i].FullName,
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
                            Tags = post.Links.Select(x => x.Categories.Title)
                                .Where(x => x != "Uncategorized")
                                .ToArray()
                        };

                        var commentsCollection = new PostComments();
                        commentsCollection.Comments = post.Comments
                            .Where(comment => comment.StatusFlag == 1)
                            .OrderBy(comment => comment.DateCreated)
                            .Select(
                                comment => new PostComments.Comment
                                    {
                                        Id = commentsCollection.GenerateNewCommentId(),
                                        Author = comment.Author,
                                        //Body = ConvertCommentToMarkdown(comment.Body),
                                        CreatedAt = comment.DateCreated,
                                        Email = comment.Email,
                                        Important = comment.IsBlogAuthor ?? false,
                                        Url = comment.Url,
                                        IsSpam = false
                                    }
                            ).ToList();
                        commentsCollection.Spam = post.Comments
                            .Where(comment => comment.StatusFlag != 1)
                            .OrderBy(comment => comment.DateCreated)
                            .Select(
                                comment => new PostComments.Comment
                                    {
                                        Id = commentsCollection.GenerateNewCommentId(),
                                        Author = comment.Author,
                                        //Body = ConvertCommentToMarkdown(comment.Body),
                                        CreatedAt = comment.DateCreated,
                                        Email = comment.Email,
                                        Important = comment.IsBlogAuthor ?? false,
                                        Url = comment.Url,
                                        IsSpam = true
                                    }
                            ).ToList();

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

        private static void GetAllHtmlFromComments()
        {
            using (var e = new SubtextEntities())
            {
                int c = 0;
                foreach (var comment in e.Comments.ToArray())
                {
                    if (IsHtmlComment(comment.Body))
                    {
                        //Console.WriteLine("Id: " + comment.Id + " Comment: " + comment.Body + Environment.NewLine + Environment.NewLine);
                        c++;
                        //if (c > 10)
                        //{
                        //    Console.WriteLine("Break: " + c);
                        //    break;
                        //}
                    }
                }
            }
        }

        static HashSet<string> seen = new HashSet<string>();

        private static bool IsHtmlComment(string body)
        {
            var match = Regex.Match(ConvertCommentToMarkdown(body), @"</[^ac]\w+>"); // <[^ =T>.</acu]
            if (match.Success)
            {
                if (seen.Add(match.Value))
                    Console.WriteLine("Match: " + match.Value);
                return true;
            }
            return false;
        }

        private static string ConvertCommentToMarkdown(string body)
        {
            return HtmlToMarkdown(body);

            body = body.Replace("<br />", "  " + Environment.NewLine);

            body = body.Replace("<strong>", "**");
            body = body.Replace("</strong>", "**");
            body = body.Replace("<b>", "**");
            body = body.Replace("</b>", "**");

            body = body.Replace("<i>", "*");
            body = body.Replace("</i>", "*");
            body = body.Replace("<em>", "*");
            body = body.Replace("</em>", "*");

            body = body.Replace("</pre>", "*");
            body = body.Replace("</quote>", "*");
            body = body.Replace("</h1>", "*");
            body = body.Replace("</a>", "*");
            body = body.Replace("</code>", "*");

            return body;
        }

        public static string HtmlToMarkdown(string html)
        {
            var xslt = new XslCompiledTransform();
            xslt.Load("markdown.xsl");

            // Execute the transform and output the results to a file.
            var buffer = new StringBuilder();
            var writer1 = new StringWriter(buffer);
            var writer = new XmlTextWriter(writer1);

            var doc = FromHtml(new StringReader(html));
            xslt.Transform(new XmlNodeReader(doc), writer);

            return buffer.ToString();
        }

        private static XmlDocument FromHtml(TextReader reader)
        {
            // setup SGMLReader
            Sgml.SgmlReader sgmlReader = new Sgml.SgmlReader();
            sgmlReader.DocType = "HTML";
            sgmlReader.WhitespaceHandling = WhitespaceHandling.All;
            sgmlReader.CaseFolding = Sgml.CaseFolding.ToLower;
            sgmlReader.InputStream = reader;

            // create document
            var doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.XmlResolver = null;
            doc.Load(sgmlReader);
            return doc;
        }

    }
}