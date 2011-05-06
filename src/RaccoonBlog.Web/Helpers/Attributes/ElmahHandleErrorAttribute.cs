using System.Web;
using System.Web.Mvc;
using Elmah;
using RaccoonBlog.Web.Infrastructure.Raven;
using RaccoonBlog.Web.Models;

namespace RaccoonBlog.Web.Helpers.Attributes
{
    public class ElmahHandleErrorAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
			
			ErrorLog.GetDefault(HttpContext.Current).Log(new Error(context.Exception, HttpContext.Current));

			context.HttpContext.Response.TrySkipIisCustomErrors = true;

			BlogConfig blogConfig;
			using(var session = DocumentStoreHolder.DocumentStore.OpenSession())
			{
				blogConfig = session.Load<BlogConfig>("Blog/Config");
			}

        	new ViewResult
			{
				ViewName = "500",
				ViewBag =
					{
						CustomCss = blogConfig.CustomCss,
						BlogTitle = blogConfig.Title,
						BlogSubtitle = blogConfig.Subtitle,
					}
			}.ExecuteResult(context);
        }
    }
}