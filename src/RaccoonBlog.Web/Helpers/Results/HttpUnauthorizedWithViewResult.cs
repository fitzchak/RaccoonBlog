namespace RaccoonBlog.Web.Helpers.Results
{
    public class HttpUnauthorizedWithViewResult : HttpStatusCodeWithViewResult
    {
        public HttpUnauthorizedWithViewResult(string statusDescription) : base(401, statusDescription) { }
    }
}