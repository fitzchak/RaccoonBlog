using RaccoonBlog.Web.Infrastructure;

namespace RaccoonBlog.Web.ViewModels
{
    public class TagsListViewModel
    {
        public string Name { get; set; }

        private string _slug;
        public string Slug
        {
            get
            {
                return _slug ?? (_slug = SlugConverter.TitleToSlug(Name));
            }
        }

        public int Count { get; set; }
    }
}
