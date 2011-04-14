namespace RavenDbBlog.Controllers
{
    public class HttpUnauthorizedWithViewResult : HttpStatusCodeWithViewResult
    {
        public HttpUnauthorizedWithViewResult(string statusDescription) : base(401, statusDescription) { }
    }
}