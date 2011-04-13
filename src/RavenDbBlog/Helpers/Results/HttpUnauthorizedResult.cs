namespace RavenDbBlog.Controllers
{
    public class HttpUnauthorizedResult : HttpStatusCodeResult
    {
        public HttpUnauthorizedResult(string statusDescription) : base(401, statusDescription) { }
    }
}