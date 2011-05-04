namespace RaccoonBlog.Web.ViewModels
{
    public class SectionDetails
    {
        public string Title { get; set; }

        public string Body { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }

        public bool IsActionSection()
        {
            return string.IsNullOrEmpty(Body);
        }
    }
}