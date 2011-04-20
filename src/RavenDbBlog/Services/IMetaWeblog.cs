using System.Security;
using CookComputing.XmlRpc;
using RavenDbBlog.Services.RssModels;

namespace RavenDbBlog.Services
{
    public interface IMetaWeblog
    {
        #region MetaWeblog API

        [XmlRpcMethod("metaWeblog.newPost")]
        string AddPost(string blogid, string username, SecureString password, Post post, bool publish);

        [XmlRpcMethod("metaWeblog.editPost")]
        bool UpdatePost(string postid, string username, SecureString password, Post post, bool publish);

        [XmlRpcMethod("metaWeblog.getPost")]
        Post GetPost(string postid, string username, SecureString password);

        [XmlRpcMethod("metaWeblog.getCategories")]
        CategoryInfo[] GetCategories(string blogid, string username, SecureString password);

        [XmlRpcMethod("metaWeblog.getRecentPosts")]
        Post[] GetRecentPosts(string blogid, string username, SecureString password, int numberOfPosts);

        [XmlRpcMethod("metaWeblog.newMediaObject")]
        MediaObjectInfo NewMediaObject(string blogid, string username, SecureString password,
            MediaObject mediaObject);

        #endregion

        #region Blogger API

        [XmlRpcMethod("blogger.deletePost")]
        [return: XmlRpcReturnValue(Description = "Returns true.")]
        bool DeletePost(string key, string postid, string username, SecureString password, bool publish);

        [XmlRpcMethod("blogger.getUsersBlogs")]
        BlogInfo[] GetUsersBlogs(string key, string username, SecureString password);

        [XmlRpcMethod("blogger.getUserInfo")]
        UserInfo GetUserInfo(string key, string username, SecureString password);

        #endregion
    }
}
