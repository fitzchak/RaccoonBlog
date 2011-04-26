using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CookComputing.XmlRpc;
using Raven.Client;
using RavenDbBlog.Indexes;
using RavenDbBlog.Infrastructure.AutoMapper.Profiles.Resolvers;
using RavenDbBlog.Infrastructure.Raven;
using System.Linq;
using RavenDbBlog.Services.RssModels;

namespace RavenDbBlog.Services
{
    public class MetaWeblog : XmlRpcService, IMetaWeblog
    {
        private readonly IDocumentSession session;
        PostSchedulingStrategy postScheduleringStrategy;

        public MetaWeblog()
        {
            session = DocumentStoreHolder.DocumentStore.OpenSession();
            postScheduleringStrategy = new PostSchedulingStrategy(session);
        }

        private UrlHelper Url
        {
            get { return new UrlHelper(new RequestContext(new HttpContextWrapper(Context), new RouteData())); }
        }

        #region IMetaWeblog Members

        string IMetaWeblog.AddPost(string blogid, string username, string password, Post post, bool publish)
        {
            ValidateUser(username, password);
            var comments = new Core.Models.PostComments();
            session.Store(comments);

            var publishDate = post.dateCreated == null
                                ? postScheduleringStrategy.Schedule()
                                : postScheduleringStrategy.Schedule(new DateTimeOffset(post.dateCreated.Value));

            var newPost = new Core.Models.Post
            {
                Author = "users/" + username,
                Body = post.description,
                CommentsId = comments.Id,
                CreatedAt = DateTimeOffset.Now,
                SkipAutoReschedule = post.dateCreated != null,
                PublishAt = publishDate,
                Tags = post.categories,
                Title = post.title,
                CommentsCount = 0,
            };
            session.Store(newPost);
            session.SaveChanges();

            return newPost.Id;
        }

        bool IMetaWeblog.UpdatePost(string postid, string username, string password,
            Post post, bool publish)
        {
            ValidateUser(username, password);
            var postToEdit = session.Load<Core.Models.Post>(postid);
            if (postToEdit == null)
                throw new XmlRpcFaultException(0, "Post does not exists");
            postToEdit.Author = "users/" + username;
            postToEdit.Body = post.description;
            if (
                // don't bother moving things if we are already talking about something that is fixed
                postToEdit.SkipAutoReschedule &&
                // if we haven't modified it, or if we modified to the same value, we can ignore this
                post.dateCreated != null &&
                post.dateCreated.Value != postToEdit.PublishAt.DateTime
                )
            {
                // schedule all the future posts up 
                postToEdit.PublishAt = postScheduleringStrategy.Schedule(new DateTimeOffset(post.dateCreated.Value));
            }
            postToEdit.Tags = post.categories;
            postToEdit.Title = post.title;

            {
                // schedule all the future posts up 
                postToEdit.PublishAt = postScheduleringStrategy.Schedule(new DateTimeOffset(post.dateCreated.Value));
            }
            postToEdit.Tags = post.categories;
            postToEdit.Title = post.title;

            session.SaveChanges();

            return true;
        }

        Post IMetaWeblog.GetPost(string postid, string username, string password)
        {
            ValidateUser(username, password);
            var thePost = session.Load<Core.Models.Post>(postid);

            return new Post
            {
                wp_slug = SlugConverter.TitleToSlag(thePost.Title),
                description = thePost.Body,
                dateCreated = thePost.PublishAt.DateTime,
                categories = thePost.Tags.ToArray(),
                title = thePost.Title,
                postid = thePost.Id,
            };
        }

        CategoryInfo[] IMetaWeblog.GetCategories(string blogid, string username, string password)
        {
            ValidateUser(username, password);
            var mostRecentTag = new DateTimeOffset(DateTimeOffset.Now.Year - 2,
                                                   DateTimeOffset.Now.Month,
                                                   1, 0, 0, 0,
                                                   DateTimeOffset.Now.Offset);

            var categoryInfos = session.Query<TagCount, Tags_Count>()
                .Where(x => x.Count > 20 && x.LastSeenAt > mostRecentTag)
                .ToList();

            return categoryInfos.Select(x => new CategoryInfo
            {
                categoryid = x.Name,
                description = x.Name,
                title = x.Name,
                htmlUrl = Url.Action("Tag", "Post", new { x.Name }),
                rssUrl = Url.Action("Tag", "Syndication", new { x.Name }),
            }).ToArray();
        }

        Post[] IMetaWeblog.GetRecentPosts(string blogid, string username, string password,
            int numberOfPosts)
        {
            ValidateUser(username, password);

            var list = session.Query<Core.Models.Post>()
                .OrderByDescending(x => x.PublishAt)
                .Take(numberOfPosts)
                .ToList();

            return list.Select(thePost => new Post
            {
                wp_slug = SlugConverter.TitleToSlag(thePost.Title),
                description = thePost.Body,
                dateCreated = thePost.PublishAt.DateTime,
                categories = thePost.Tags.ToArray(),
                title = thePost.Title,
                postid = thePost.Id,
            }).ToArray();
        }

        MediaObjectInfo IMetaWeblog.NewMediaObject(string blogid, string username, string password,
            MediaObject mediaObject)
        {
            ValidateUser(username, password);
            var imagePhysicalPath = Context.Server.MapPath(ConfigurationManager.AppSettings["uploadsPath"]);
            var imageWebPath = ConfigurationManager.AppSettings["UploadsPath"].Replace("~", Context.Request.ApplicationPath);

            imagePhysicalPath = Path.Combine(imagePhysicalPath, mediaObject.name);
            var directoryPath = Path.GetDirectoryName(imagePhysicalPath).Replace("/", "\\");
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            File.WriteAllBytes(imagePhysicalPath, mediaObject.bits);


            return new MediaObjectInfo()
            {
                url = Path.Combine(imageWebPath, mediaObject.name)
            };
        }

        bool IMetaWeblog.DeletePost(string key, string postid, string username, string password, bool publish)
        {
            ValidateUser(username, password);
            var thePost = session.Load<Core.Models.Post>(postid);

            if (thePost != null)
            {
                var postComments = session.Load<Core.Models.PostComments>(thePost.CommentsId);
                if (postComments != null)
                    session.Delete(postComments);
                session.Delete(thePost);
            }

            session.SaveChanges();

            return true;
        }

        BlogInfo[] IMetaWeblog.GetUsersBlogs(string key, string username, string password)
        {
            ValidateUser(username, password);
            return new[]
			{
				new BlogInfo
				{
					blogid = "blogs/1",
					blogName = username,
					url = Context.Request.RawUrl
				},
			};
        }

        UserInfo IMetaWeblog.GetUserInfo(string key, string username, string password)
        {
            ValidateUser(username, password);
            return new UserInfo
            {
                email = "none",
                nickname = username,
                userid = "users/" + username
            };
        }

        #endregion

        #region Private Methods

        private void ValidateUser(string username, string password)
        {
            var user = session.Load<Core.Models.User>("users/" + username);
            if (user == null || user.ValidatePassword(password))
                throw new XmlRpcFaultException(0, "User is not valid!");
            if (user.Enabled == false)
                throw new XmlRpcFaultException(0, "User is not enabled!");

        }

        #endregion
    }
}
