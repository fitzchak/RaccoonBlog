/*using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CookComputing.XmlRpc;
using Impleo.Web.Model;
using NHibernate;
using NHibernate.Criterion;
using Impleo.Web.Extensions;
using Transaction = Impleo.Web.Infrastructure.Transaction;

namespace Impleo.Web.Services
{
    [XmlRpcService(
        Name = "MetaWeblogAPI",
        Description = "CMS using meta weblog API",
        AutoDocumentation = true)]
    public class MetaWeblog : XmlRpcService, IMetaWeblog
    {
        private readonly IUsersService userService;
        private readonly ISession session;

        public MetaWeblog()
            : this(DependencyResolver.Current.GetService<IUsersService>(), DependencyResolver.Current.GetService<ISession>())
        {

        }

        public MetaWeblog(IUsersService userService, ISession session)
        {
            this.userService = userService;
            this.session = session;
        }

        public bool editPost(string postid, string username, string password, Post post, bool publish)
        {
            Transaction.Execute(() => SaveEditedEntry(postid, username, password, post, publish));
            return true;
        }

        private void SaveEditedEntry(string postid, string username, string password, Post post, bool publish)
        {
            ValidateUser(username, password, post.wp_slug);
            var entry = session.Get<Entry>(int.Parse(postid));
            if (entry == null)
                throw new XmlRpcFaultException(0, "Entry does not exists " + postid);

            var old = new OldEntry(entry);
            session.Save(old);

            entry.Categories.Clear();
            SetCategories(session, post, entry);

            entry.Body = post.description;
            entry.Title = post.title;
            entry.Author = username;
            entry.DateSyndicated = post.dateCreated ?? entry.DateSyndicated;
            entry.IsActive = publish;
            entry.DateModified = DateTime.UtcNow;
            entry.Slug = post.wp_slug;

            ValidateEntrySlug(entry);
            GenerateMenuItemsIfNeeded(entry);
            GarbageCollectMenuItems();
        }

        private void ValidateUser(string username, string password, string slug)
        {
            if (string.IsNullOrEmpty(slug))
                throw new XmlRpcException("When using Impleo, slug is mandatory");
            var results = userService.ValidateUserForSlug(username, password, slug);
            if (results == ValidationResults.UserDoesNotExistsOrBadPassword)
                throw new HttpException(403, "could not validate username or password");

            if (results == ValidationResults.NoPermissionsForPath)
                throw new HttpException(403, "you do not have permissions to edit " + slug);
        }

        private void ValidateUser(string username, string password)
        {
            if (userService.ValidateUser(username, password) == false)
                throw new HttpException(403, "could not validate username or password");

        }

        public CategoryInfo[] getCategories(object blogid, string username, string password)
        {
            return Transaction.Execute(() =>
            {
                ValidateUser(username, password);

                var categories = session.CreateCriteria<Model.Category>()
                    .List<Model.Category>()
                    .Select(c => new CategoryInfo
                    {
                        categoryid = c.Id.ToString(),
                        title = c.Name,
                        description = c.Name,
                        htmlUrl = Context.Request.ApplicationPath + "categories/" + c.Name,
                        rssUrl = Context.Request.ApplicationPath + "categories/" + c.Name + "/rss",
                    }).ToArray();

                return categories;
            });
        }

        public Post getPost(string postid, string username, string password)
        {
            return Transaction.Execute(() =>
            {
                ValidateUser(username, password);
                var entry = session.Get<Entry>(int.Parse(postid));
                if (entry == null)
                    throw new XmlRpcFaultException(0, "Entry does not exists: " + postid);

                var post = new Post
                {
                    link = Context.Request.ApplicationPath + entry.Slug + "/" + entry.Id,
                    description = entry.Body,
                    dateCreated = entry.DateCreated,
                    postid = entry.Id.ToString(),
                    title = entry.Title,
                    permalink = Context.Request.ApplicationPath + entry.Slug + "/" + entry.Id,
                    categories = entry.Categories.Select(x => x.Name).ToArray(),
                    wp_slug = entry.Slug
                };

                return post;
            });
        }

        public Post[] getRecentPosts(object blogid, string username, string password, int numberOfPosts)
        {
            return Transaction.Execute(() =>
            {
                ValidateUser(username, password);

                ICollection<Entry> entries = session.CreateCriteria<Entry>()
                    .SetMaxResults(numberOfPosts)
                    .List<Entry>();

                var posts = (from entry in entries
                             select new Post
                             {
                                 dateCreated = entry.DateCreated,
                                 description = entry.Body,
                                 link = Context.Request.ApplicationPath + entry.Slug + "/" + entry.Id,
                                 permalink = Context.Request.ApplicationPath + entry.Slug + "/" + entry.Id,
                                 title = entry.Title,
                                 postid = entry.Id.ToString(),
                                 userid = username,
                                 wp_slug = entry.Slug,
                                 categories = entry.Categories.Select(x => x.Name).ToArray()
                             }).ToArray();

                return posts;
            });
        }

        public string newPost(object blogid, string username, string password, Post post, bool publish)
        {
            return Transaction.Execute(() =>
            {
                // this is required to support the new post that WLW generate to read the site template
                var isWlwStylePost = post.title.StartsWith(Constants.WindowsLiveWriterStylePostPrefix);
                var slug = isWlwStylePost ? Constants.WindowsLiveWriterThemeSlug : post.wp_slug;

                ValidateUser(username, password, slug);

                var existingEntry = session.CreateCriteria<Entry>()
                    .Add(Restrictions.Eq("Slug", slug))
                    .UniqueResult<Entry>();

                if (existingEntry != null)
                {
                    post.postid = existingEntry.Id.ToString();
                    SaveEditedEntry(existingEntry.Id.ToString(), username, password, post, publish);
                    return existingEntry.Id.ToString();
                }
                var entry = new Entry
                {
                    IsActive = publish,
                    Author = username,
                    Body = post.description,
                    Title = post.title,
                    DateSyndicated = post.dateCreated ?? DateTime.UtcNow,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    Slug = slug
                };

                ValidateEntrySlug(entry);
                if (isWlwStylePost == false)
                {
                    GenerateMenuItemsIfNeeded(entry);
                }
                SetCategories(session, post, entry);

                session.Save(entry);
                GarbageCollectMenuItems();
                return entry.Id.ToString();
            });
        }

        private void GarbageCollectMenuItems()
        {
            int numOfResults;
            do
            {
                numOfResults = session.CreateQuery(
                    @"delete from MenuItem mi 
                        where not exists (select 1 from Entry e where e.Slug = mi.Name) 
                        and   not exists (select 1 from MenuItem c where c.Parent.Id = mi.Id)")
                    .ExecuteUpdate();
            } while (numOfResults != 0);
        }

        private void GenerateMenuItemsIfNeeded(Entry entry)
        {
            var names = entry.Slug.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            var rootName = "/" + names[0];
            var menuItem = session.CreateCriteria<MenuItem>()
                .Add(Restrictions.Eq("Name", rootName))
                .UniqueResult<MenuItem>();

            if (menuItem == null) // we need to create the root
            {
                menuItem = new MenuItem
                {
                    Name = rootName
                };
                session.Save(menuItem);
            }
            for (int i = 1; i < names.Length; i++)
            {
                var childName = rootName + "/" + names[i];
                var childItem = session.CreateCriteria<MenuItem>()
                    .Add(Restrictions.Eq("Name", childName))
                    .UniqueResult<MenuItem>();

                if (childItem == null)
                {
                    childItem = new MenuItem
                    {
                        Name = childName,
                        Parent = menuItem,
                        Depth = menuItem.Depth + 1
                    };
                    menuItem.Children.Add(childItem);
                    session.Save(childItem);
                }
                menuItem = childItem;
                rootName = childName;
            }
        }

        private static void ValidateEntrySlug(Entry entry)
        {
            if (string.IsNullOrEmpty(entry.Slug))
                throw new XmlRpcException("slug is mandatory");
            if (entry.Slug.StartsWith("/") == false)
                throw new XmlRpcException("Slug must start with '/'");

            if (entry.Slug.Length == 1)
                throw new XmlRpcException("Slug must not be a single character long");

            if (entry.Slug.Contains("//"))
                throw new XmlRpcException("Slug must not contains '//'");

            if (entry.Slug.EndsWith("/"))
                throw new XmlRpcException("Slug cannot end with '/'");
        }

        public mediaObjectInfo newMediaObject(object blogid, string username, string password, mediaObject mediaobject)
        {
            return Transaction.Execute(() =>
            {

                ValidateUser(username, password);

                HttpContext httpContext = HttpContext.Current;
                var imagePhysicalPath = httpContext.Server.MapPath(ConfigurationManager.AppSettings["uploadsPath"]);
                var imageWebPath = ConfigurationManager.AppSettings["uploadsPath"].Replace("~",
                                                                                           httpContext.Request.ApplicationPath);

                imagePhysicalPath = Path.Combine(imagePhysicalPath, mediaobject.name);
                var directoryPath = Path.GetDirectoryName(imagePhysicalPath).Replace("/", "\\");
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
                File.WriteAllBytes(imagePhysicalPath, mediaobject.bits);


                return new mediaObjectInfo
                {
                    url = Path.Combine(imageWebPath, mediaobject.name)
                };
            });
        }

        public bool deletePost(string appKey, string postid, string username, string password, bool publish)
        {
            return Transaction.Execute(() =>
             {
                 ValidateUser(username, password);
                 var entry = session.Get<Entry>(Int32.Parse(postid));
                 ValidateUser(username, password, entry.Slug);

                 session.Delete(entry);
                 return true;
             });
        }

        public BlogInfo[] getUsersBlogs(string appKey, string username, string password)
        {
            return Transaction.Execute(() =>
            {
                ValidateUser(username, password);

                var blog = new BlogInfo
                {
                    blogid = "default-blog",
                    blogName = ConfigurationManager.AppSettings["title"],
                    url = Context.Request.ApplicationPath
                };

                return new[] { blog };
            });
        }


        public int newCategory(string blogid, string username, string password, WordpressCategory category)
        {
            return Transaction.Execute(() =>
            {
                ValidateUser(username, password);
                var newCategory = new Model.Category
                {
                    Name = category.name,
                };


                session.Save(newCategory);
                return newCategory.Id;
            });
        }

        private static void SetCategories(ISession session, Post post, Entry entry)
        {
            if (post.categories != null)
            {
                foreach (var category in session.CreateCriteria<Model.Category>()
                        .Add(Restrictions.In("Name", post.categories))
                        .List<Model.Category>())
                {
                    entry.Categories.Add(category);
                }
            }
        }
    }
}*/