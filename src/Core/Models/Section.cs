namespace RavenDbBlog.Core.Models
{
    /*Section can contains:
     * 1. Body = "Any html text"
     * 2. Can point to any internal action.
     */
    public class Section
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}