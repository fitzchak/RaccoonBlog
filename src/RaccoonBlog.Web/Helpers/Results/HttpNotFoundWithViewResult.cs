namespace RaccoonBlog.Web.Helpers.Results
{
    public class HttpNotFoundWithViewResult : HttpStatusCodeWithViewResult
    {
        public HttpNotFoundWithViewResult() : this(null) { }

        public HttpNotFoundWithViewResult(string statusDescription) : base(404, statusDescription) { }

    }
}