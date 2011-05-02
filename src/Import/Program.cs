using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Raven.Client;
using Raven.Client.Document;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Infrastructure.EntityFramework;
using Sgml;
using Post = RavenDbBlog.Infrastructure.EntityFramework.Post;
using RavenPost = RavenDbBlog.Core.Models.Post;

namespace RavenDbBlog.Import
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (IDocumentStore store = new DocumentStore
                {
                    Url = "http://localhost:8080",
                }.Initialize())
            {
                CreateSections(store);
                ImportDatabase(store);
            }
        }

        private static void CreateSections(IDocumentStore store)
        {
            Console.WriteLine("Creating sections");
            using (IDocumentSession s = store.OpenSession())
            {
                var sections = new[]
                    {
                        new Section {Title = "Administration Panel", ControllerName = "Login", ActionName = "AdministrationPanel"},
                        new Section {Title = "Title", Body = string.Format("Text 1{0}{0}Text 2{0}{0}", Environment.NewLine)},
                        new Section {Title = "Recent Comments", Body = "Recent Comments"},
                        new Section {Title = "Tags", ControllerName = "Post", ActionName = "TagsList"},
                        new Section {Title = "Archive", ControllerName = "Post", ActionName = "ArchivesList"},
                        new Section {Title = "Login", ControllerName = "Login", ActionName = "CurrentUser"},

                    };

                foreach (var section in sections)
                {
                    section.IsActive = true;
                    s.Store(section);
                }
                s.SaveChanges();
            }
            Console.WriteLine("Finish creating sections");
        }

        private static void ImportDatabase(IDocumentStore store)
        {
            Stopwatch sp = Stopwatch.StartNew();

            using (var e = new SubtextEntities())
            {
                Console.WriteLine("Starting...");

                IOrderedEnumerable<Post> theEntireDatabaseOhMygod = e.Posts
                    .Include("Comments")
                    .Include("Links")
                    .Include("Links.Categories")
                    .ToList()
                    .OrderBy(x => x.DateSyndicated);

                Console.WriteLine("Loading data took {0:#,#} ms", sp.ElapsedMilliseconds);

                using (IDocumentSession s = store.OpenSession())
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

                foreach (Post post in theEntireDatabaseOhMygod)
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
                                    Body = ConvertCommentToMarkdown(comment.Body),
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
                                    Body = ConvertCommentToMarkdown(comment.Body),
                                    CreatedAt = comment.DateCreated,
                                    Email = comment.Email,
                                    Important = comment.IsBlogAuthor ?? false,
                                    Url = comment.Url,
                                    IsSpam = true
                                }
                        ).ToList();

                    using (IDocumentSession s = store.OpenSession())
                    {
                        s.Store(commentsCollection);
                        ravenPost.CommentsId = commentsCollection.Id;

                        s.Store(ravenPost);

                        s.SaveChanges();
                    }
                }
            }
            Console.WriteLine(sp.Elapsed);
        }

        private static string ConvertCommentToMarkdown(string body)
        {
            var sb = new StringBuilder();

            var sgmlReader = new SgmlReader
                {
                    InputStream = new StringReader(body),
                    DocType = "HTML",
                    WhitespaceHandling = WhitespaceHandling.Significant,
                    CaseFolding = CaseFolding.ToLower
                };

            bool outputEndElement = false;
            int indentLevel = 0;
            while (sgmlReader.Read())
            {
                switch (sgmlReader.NodeType)
                {
                    case XmlNodeType.Text:
                        if (indentLevel > 0)
                            sb.Append("\t");
                        sb.AppendLine(sgmlReader.Value);
                        break;
                    case XmlNodeType.Element:
                        switch (sgmlReader.LocalName)
                        {
                            case "h1":
                                sb.Append("## ");
                                break;
                            case "br":
                                sb.AppendLine("  ");
                                break;
                            case "a":
                                if (sgmlReader.MoveToAttribute("href"))
                                {
                                    string url = sgmlReader.Value;
                                    sgmlReader.Read();

                                    sb.AppendFormat("[{0}]({1})", sgmlReader.Value, url);
                                }
                                break;
                            case "html":
                                break;
                            case "strong":
                            case "b":
                                sb.AppendFormat("**{0}**", sgmlReader.Value);
                                break;
                            case "i":
                            case "em":
                                sb.AppendFormat("_{0}_", sgmlReader.Value);
                                break;
                            case "li":
                                sb.AppendFormat("- {0}", sgmlReader.Value);
                                break;
                            case "pre":
                            case "code":
                            case "quote":
                                indentLevel = 1;
                                break;
                            case "ul":
                            case "ol":
                            case "img":
                                break;
                            default:
                                Console.WriteLine(sgmlReader.LocalName);
                                outputEndElement = true;
                                sb.Append("<").Append(sgmlReader.LocalName);
                                break;
                        }
                        break;
                    case XmlNodeType.SignificantWhitespace:
                    case XmlNodeType.Whitespace:
                    case XmlNodeType.CDATA:
                        break;
                    case XmlNodeType.EndElement:
                        indentLevel = 0;
                        if (outputEndElement)
                            sb.Append(">");
                        outputEndElement = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return sb.ToString();
        }
    }
}