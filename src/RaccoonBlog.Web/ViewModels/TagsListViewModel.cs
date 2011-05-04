using RavenDbBlog.Core;

namespace RavenDbBlog.ViewModels
{
    public class TagsListViewModel
    {
        public string Name { get; set; }

        private string _slug;
        public string Slug
        {
            get
            {
                return _slug ?? (_slug = SlugConverter.TitleToSlag(Name));
            }
        }

        public int Count { get; set; }
    }
}