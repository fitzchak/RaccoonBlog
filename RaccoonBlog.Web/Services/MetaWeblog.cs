using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CookComputing.XmlRpc;
using RaccoonBlog.Web.Infrastructure.Common;
using RaccoonBlog.Web.Infrastructure.Indexes;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.Services.RssModels;
using System.Linq;
using Post = RaccoonBlog.Web.Services.RssModels.Post;

namespace RaccoonBlog.Web.Services
{
    public class MetaWeblog : XmlRpcService, IMetaWeblog
    {
        private UrlHelper Url
        {
            get { return new UrlHelper(new RequestContext(new HttpContextWrapper(Context), new RouteData())); }
        }

        #region IMetaWeblog Members

        string IMetaWeblog.AddPost(string blogid, string username, string password, Post post, bool publish)
        {
        	Models.Post newPost;
			using (var session = MvcApplication.DocumentStore.OpenSession())
			{
				var user = ValidateUser(username, password);
				var comments = new PostComments
				               	{
				               		Comments = new List<PostComments.Comment>(),
				               		Spam = new List<PostComments.Comment>()
				               	};
				session.Store(comments);

				var postScheduleringStrategy = new PostSchedulingStrategy(session, DateTimeOffset.Now);
				var publishDate = post.dateCreated == null
				                  	? postScheduleringStrategy.Schedule()
				                  	: postScheduleringStrategy.Schedule(new DateTimeOffset(post.dateCreated.Value));

				newPost = new Models.Post
				              	{
				              		AuthorId = user.Id,
				              		Body = post.description,
				              		CommentsId = comments.Id,
				              		CreatedAt = DateTimeOffset.Now,
				              		SkipAutoReschedule = post.dateCreated != null,
				              		PublishAt = publishDate,
				              		Tags = post.categories,
				              		Title = post.title,
				              		CommentsCount = 0,
				              		AllowComments = true,
				              	};
				session.Store(newPost);
				comments.Post = new PostComments.PostReference
				                	{
				                		Id = newPost.Id,
				                		PublishAt = publishDate
				                	};

				session.SaveChanges();
			}

        	return newPost.Id;
        }

        bool IMetaWeblog.UpdatePost(string postid, string username, string password, Post post, bool publish)
        {
			using (var session = MvcApplication.DocumentStore.OpenSession())
			{
				var user = ValidateUser(username, password);
				var postToEdit = session
					.Include<Models.Post>(x => x.CommentsId)
					.Load(postid);
				if (postToEdit == null)
					throw new XmlRpcFaultException(0, "Post does not exists");

				if (string.IsNullOrEmpty(postToEdit.AuthorId))
					postToEdit.AuthorId = user.Id;
				else
				{
					postToEdit.LastEditedByUserId = user.Id;
					postToEdit.LastEditedAt = DateTimeOffset.Now;
				}

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
					var postScheduleringStrategy = new PostSchedulingStrategy(session, DateTimeOffset.Now);
					postToEdit.PublishAt = postScheduleringStrategy.Schedule(new DateTimeOffset(post.dateCreated.Value));
					session.Load<PostComments>(postToEdit.CommentsId).Post.PublishAt = postToEdit.PublishAt;
				}
				postToEdit.Tags = post.categories;
				postToEdit.Title = post.title;

				session.SaveChanges();
			}

        	return true;
        }

        Post IMetaWeblog.GetPost(string postid, string username, string password)
        {
            ValidateUser(username, password);

			using (var session = MvcApplication.DocumentStore.OpenSession())
			{
				var thePost = session.Load<Models.Post>(postid);
				if (thePost.IsDeleted)
				{
					throw new InvalidOperationException("You cannot get deleted post");
				}

				return new Post
				       	{
				       		wp_slug = SlugConverter.TitleToSlug(thePost.Title),
				       		description = thePost.Body,
				       		dateCreated = thePost.PublishAt.DateTime,
				       		categories = thePost.Tags.ToArray(),
				       		title = thePost.Title,
				       		postid = thePost.Id,
				       	};
			}
        }

        CategoryInfo[] IMetaWeblog.GetCategories(string blogid, string username, string password)
        {
            ValidateUser(username, password);
            var mostRecentTag = new DateTimeOffset(DateTimeOffset.Now.Year - 2,
                                                   DateTimeOffset.Now.Month,
                                                   1, 0, 0, 0,
                                                   DateTimeOffset.Now.Offset);

			using (var session = MvcApplication.DocumentStore.OpenSession())
			{
				var categoryInfos = session.Query<Tags_Count.ReduceResult, Tags_Count>()
					.Where(x => x.LastSeenAt > mostRecentTag)
					.ToList();

				return categoryInfos.Select(x => new CategoryInfo
				                                 	{
				                                 		categoryid = x.Name,
				                                 		description = x.Name,
				                                 		title = x.Name,
				                                 		htmlUrl = Url.Action("Tag", "Post", new {slug = x.Name}),
				                                 		rssUrl = Url.Action("Rss", "Syndication", new {tag = x.Name}),
				                                 	}).ToArray();
			}
        }

        Post[] IMetaWeblog.GetRecentPosts(string blogid, string username, string password,
            int numberOfPosts)
        {
            ValidateUser(username, password);

			using (var session = MvcApplication.DocumentStore.OpenSession())
			{
				var list = session.Query<Models.Post>()
					.Where(p => p.IsDeleted == false)
					.OrderByDescending(x => x.PublishAt)
					.Take(numberOfPosts)
					.ToList();

				return list.Select(thePost => new Post
				                              	{
				                              		wp_slug = SlugConverter.TitleToSlug(thePost.Title),
				                              		description = thePost.Body,
				                              		dateCreated = thePost.PublishAt.DateTime,
				                              		categories = thePost.Tags.ToArray(),
				                              		title = thePost.Title,
				                              		postid = thePost.Id,
				                              	}).ToArray();
			}
        }


        MediaObjectInfo IMetaWeblog.NewMediaObject(string blogid, string username, string password,
            MediaObject mediaObject)
        {
            ValidateUser(username, password);
            var imagePhysicalPath = Context.Server.MapPath(ConfigurationManager.AppSettings["uploadsPath"]);
            var imageWebPath = VirtualPathUtility.ToAbsolute(ConfigurationManager.AppSettings["UploadsPath"]);

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

		int IMetaWeblog.newCategory(string blogid, string username, string password, WordpressCategory category)
    	{
			ValidateUser(username, password);
			return 1;// we don't support explicit categories
    	}

    	bool IMetaWeblog.DeletePost(string key, string postid, string username, string password, bool publish)
        {
            ValidateUser(username, password);

			using (var session = MvcApplication.DocumentStore.OpenSession())
			{
				var thePost = session.Load<Models.Post>(postid);

				if (thePost != null)
				{
					thePost.IsDeleted = true;
				}

				session.SaveChanges();
			}

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
            var user = ValidateUser(username, password);
            return new UserInfo
            {
                email = user.Email,
                nickname = user.FullName,
                firstname = user.FullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(),
                lastname = user.FullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault(),
                userid = user.Id
            };
        }

        #endregion

        #region Private Methods

        private User ValidateUser(string username, string password)
        {
        	User user;
			using (var session = MvcApplication.DocumentStore.OpenSession())
			{
				user = session.GetUserByEmail(username);
			}

        	if (user == null || user.ValidatePassword(password) == false)
                throw new XmlRpcFaultException(0, "User is not valid!");
            if (user.Enabled == false)
                throw new XmlRpcFaultException(0, "User is not enabled!");
            return user;
        }

        #endregion
    }
}
