using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using CookComputing.XmlRpc;
using Raven.Client;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Indexes;
using RavenDbBlog.Infrastructure.Raven;
using System.Linq;

namespace RavenDbBlog.Services
{
    public class MetaWeblog : XmlRpcService, IMetaWeblog
    {
    	private readonly IDocumentSession session = DocumentStoreHolder.DocumentStore.OpenSession();

        #region IMetaWeblog Members

        string IMetaWeblog.AddPost(string blogid, string username, string password, Post post, bool publish)
        {
			if (ValidateUser(username, password))
			{
				var comments = new PostComments();
				session.Store(comments);

				var newPost = new Core.Models.Post
				{
					Author = "users/" + username,
					Body = post.description,
					CommentsId = comments.Id,
					CreatedAt = DateTimeOffset.Now,
					//TODO: Smarter re-scheduling
					PublishAt = post.dateCreated == null ? DateTimeOffset.Now : new DateTimeOffset(post.dateCreated.Value),
					Slug = post.wp_slug,
					Tags = post.categories,
					Title = post.title,
					CommentsCount = 0,
				};
				session.Store(newPost);
				session.SaveChanges();
                
                return newPost.Id;
            }
            throw new XmlRpcFaultException(0, "User is not valid!");
        }

        bool IMetaWeblog.UpdatePost(string postid, string username, string password,
            Post post, bool publish)
        {
            if (ValidateUser(username, password))
            {
            	var postToEdit = session.Load<Core.Models.Post>(postid);
				if (postToEdit == null)
					throw new XmlRpcFaultException(0, "Post does not exists");
            	postToEdit.Slug = post.wp_slug;
            	postToEdit.Author = "users/" + username;
            	postToEdit.Body = post.description;
				if(post.dateCreated!=null)
					postToEdit.PublishAt = new DateTimeOffset(post.dateCreated.Value);
            	postToEdit.Tags = post.categories;
            	postToEdit.Title = post.title;

				session.SaveChanges();

            	return true;
            }
            throw new XmlRpcFaultException(0, "User is not valid!");
        }

        Post IMetaWeblog.GetPost(string postid, string username, string password)
        {
            if (ValidateUser(username, password))
            {
				var thePost = session.Load<Core.Models.Post>(postid);

				if (thePost == null)
					throw new XmlRpcFaultException(0, "Post does not exists");

            	return new Post
            	{
            		wp_slug = thePost.Slug,
            		description = thePost.Body,
            		dateCreated = thePost.PublishAt.DateTime,
            		categories = thePost.Tags.ToArray(),
            		title = thePost.Title,
					postid = thePost.Id,
            	};
            }
            throw new XmlRpcFaultException(0, "User is not valid!");
        }

        CategoryInfo[] IMetaWeblog.GetCategories(string blogid, string username, string password)
        {
            if (ValidateUser(username, password))
            {
            	return session.Query<TagCount, Tags_Count>()
            		.Select(x => new CategoryInfo
            		{
            			categoryid = x.Name,
            			description = x.Name,
            			title = x.Name
            		}).ToArray();
            }
            throw new XmlRpcFaultException(0, "User is not valid!");
        }

        Post[] IMetaWeblog.GetRecentPosts(string blogid, string username, string password,
            int numberOfPosts)
        {
            if (ValidateUser(username, password))
            {
                
            	var list = session.Query<Core.Models.Post>()
            		.OrderByDescending(x=>x.PublishAt)
            		.Take(numberOfPosts)
            		.ToList();

            	return list.Select(thePost => new Post
            	{
            		wp_slug = thePost.Slug,
            		description = thePost.Body,
            		dateCreated = thePost.PublishAt.DateTime,
            		categories = thePost.Tags.ToArray(),
            		title = thePost.Title,
            		postid = thePost.Id,
            	}).ToArray();
            }
            throw new XmlRpcFaultException(0, "User is not valid!");
        }

        MediaObjectInfo IMetaWeblog.NewMediaObject(string blogid, string username, string password,
            MediaObject mediaObject)
        {
            if (ValidateUser(username, password))
            {
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
            throw new XmlRpcFaultException(0, "User is not valid!");
        }

        bool IMetaWeblog.DeletePost(string key, string postid, string username, string password, bool publish)
        {
            if (ValidateUser(username, password))
            {
				var thePost = session.Load<Core.Models.Post>(postid);

				if(thePost != null)
				{
					var postComments = session.Load<Core.Models.PostComments>(thePost.CommentsId);
					if(postComments!=null)
						session.Delete(postComments);
					session.Delete(thePost);
				}

				session.SaveChanges();

            	return true;
            }
            throw new XmlRpcFaultException(0, "User is not valid!");
        }

        BlogInfo[] IMetaWeblog.GetUsersBlogs(string key, string username, string password)
        {
            if (ValidateUser(username, password))
            {
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
            throw new XmlRpcFaultException(0, "User is not valid!");
        }

        UserInfo IMetaWeblog.GetUserInfo(string key, string username, string password)
        {
            if (ValidateUser(username, password))
            {
                return new UserInfo
                {
                	email = "none",
					nickname = username,
					userid = "users/"+username
                };
            }
            throw new XmlRpcFaultException(0, "User is not valid!");
        }

        #endregion

        #region Private Methods

        private bool ValidateUser(string username, string password)
        {
        	var user = session.Load<User>("users/"+username);
			if (user == null)
				return false;

			if(DateTime.Now > new DateTime(2011, 4, 25))
				throw new InvalidOperationException("Hack expired, fix me already");

        	return user.ValidatePassword(password);
        }

    	#endregion
    }
}
