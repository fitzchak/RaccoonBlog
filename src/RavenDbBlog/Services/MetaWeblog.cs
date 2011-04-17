using CookComputing.XmlRpc;
using System.Collections.Generic;

namespace RavenDbBlog.Services
{
    public class MetaWeblog : XmlRpcService, IMetaWeblog
    {
        #region Public Constructors

        public MetaWeblog()
        {
        }

        #endregion

        #region IMetaWeblog Members

        string IMetaWeblog.AddPost(string blogid, string username, string password,
            Post post, bool publish)
        {
            if (ValidateUser(username, password))
            {
                string id = string.Empty;

                // TODO: Implement your own logic to add the post and set the id

                return id;
            }
            throw new XmlRpcFaultException(0, "User is not valid!");
        }

        bool IMetaWeblog.UpdatePost(string postid, string username, string password,
            Post post, bool publish)
        {
            if (ValidateUser(username, password))
            {
                bool result = false;

                // TODO: Implement your own logic to add the post and set the result

                return result;
            }
            throw new XmlRpcFaultException(0, "User is not valid!");
        }

        Post IMetaWeblog.GetPost(string postid, string username, string password)
        {
            if (ValidateUser(username, password))
            {
                Post post = new Post();

                // TODO: Implement your own logic to update the post and set the post

                return post;
            }
            throw new XmlRpcFaultException(0, "User is not valid!");
        }

        CategoryInfo[] IMetaWeblog.GetCategories(string blogid, string username, string password)
        {
            if (ValidateUser(username, password))
            {
                List<CategoryInfo> categoryInfos = new List<CategoryInfo>();

                // TODO: Implement your own logic to get category info and set the categoryInfos

                return categoryInfos.ToArray();
            }
            throw new XmlRpcFaultException(0, "User is not valid!");
        }

        Post[] IMetaWeblog.GetRecentPosts(string blogid, string username, string password,
            int numberOfPosts)
        {
            if (ValidateUser(username, password))
            {
                List<Post> posts = new List<Post>();

                // TODO: Implement your own logic to get posts and set the posts

                return posts.ToArray();
            }
            throw new XmlRpcFaultException(0, "User is not valid!");
        }

        MediaObjectInfo IMetaWeblog.NewMediaObject(string blogid, string username, string password,
            MediaObject mediaObject)
        {
            if (ValidateUser(username, password))
            {
                MediaObjectInfo objectInfo = new MediaObjectInfo();

                // TODO: Implement your own logic to add media object and set the objectInfo

                return objectInfo;
            }
            throw new XmlRpcFaultException(0, "User is not valid!");
        }

        bool IMetaWeblog.DeletePost(string key, string postid, string username, string password, bool publish)
        {
            if (ValidateUser(username, password))
            {
                bool result = false;

                // TODO: Implement your own logic to delete the post and set the result

                return result;
            }
            throw new XmlRpcFaultException(0, "User is not valid!");
        }

        BlogInfo[] IMetaWeblog.GetUsersBlogs(string key, string username, string password)
        {
            if (ValidateUser(username, password))
            {
                List<BlogInfo> infoList = new List<BlogInfo>();

                // TODO: Implement your own logic to get blog info objects and set the infoList

                return infoList.ToArray();
            }
            throw new XmlRpcFaultException(0, "User is not valid!");
        }

        UserInfo IMetaWeblog.GetUserInfo(string key, string username, string password)
        {
            if (ValidateUser(username, password))
            {
                UserInfo info = new UserInfo();

                // TODO: Implement your own logic to get user info objects and set the info

                return info;
            }
            throw new XmlRpcFaultException(0, "User is not valid!");
        }

        #endregion

        #region Private Methods

        private bool ValidateUser(string username, string password)
        {
            bool result = false;

            // TODO: Implement the logic to validate the user

            return result;
        }

        #endregion
    }
}
