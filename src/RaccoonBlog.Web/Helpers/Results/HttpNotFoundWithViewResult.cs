namespace RavenDbBlog.Controllers
{
    public class HttpNotFoundWithViewResult : HttpStatusCodeWithViewResult
    {
        public HttpNotFoundWithViewResult() : this(null) { }

        public HttpNotFoundWithViewResult(string statusDescription) : base(404, statusDescription) { }

    }
}