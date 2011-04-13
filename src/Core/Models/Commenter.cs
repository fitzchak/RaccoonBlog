using System;

namespace RavenDbBlog.Core.Models
{
    public class Commenter
    {
        public string Id { get; set; }
        public Guid Key { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
    }
}