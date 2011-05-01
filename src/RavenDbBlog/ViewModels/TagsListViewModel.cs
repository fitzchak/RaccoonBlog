using RavenDbBlog.Core;

namespace RavenDbBlog.ViewModels
{
    public class TagsListViewModel
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                Slug = SlugConverter.TitleToSlag(Name);
            }
        }

        public string Slug { get; private set; }
        public int Count { get; set; }
    }
}