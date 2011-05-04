namespace RavenDbBlog.ViewModels
{
    public class EditPostViewModel
    {
        private PostInput _input;
        public PostInput Input
        {
            get { return _input ?? (_input = new PostInput()); }
            set { _input = value; }
        }
    }
}