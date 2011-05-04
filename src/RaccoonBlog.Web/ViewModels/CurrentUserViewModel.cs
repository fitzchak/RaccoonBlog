namespace RavenDbBlog.ViewModels
{
    public class CurrentUserViewModel
    {
        public string FullName { get; set; }

        public bool IsAuthenticated()
        {
            return string.IsNullOrEmpty(FullName) == false;
        }
    }
}