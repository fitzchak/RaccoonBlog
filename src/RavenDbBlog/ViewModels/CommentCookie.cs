using System;

namespace RavenDbBlog.ViewModels
{
    [Serializable]
    public class CommentCookie
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public bool? RememberMe { get; set; }
    }
}