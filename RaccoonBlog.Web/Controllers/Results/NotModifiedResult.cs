using Microsoft.AspNetCore.Mvc;

namespace RaccoonBlog.Web.Controllers.Results
{
    public class NotModifiedResult : StatusCodeResult
    {
        /// <summary>
        /// Creates a new <see cref="T:Microsoft.AspNetCore.Mvc.NotFoundResult" /> instance.
        /// </summary>
        public NotModifiedResult()
            : base(304)
        {
        }
    }
}