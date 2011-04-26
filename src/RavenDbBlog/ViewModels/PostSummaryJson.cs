using System;

namespace RavenDbBlog.ViewModels
{
    public class PostSummaryJson
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Start { get; set; }
    }
}