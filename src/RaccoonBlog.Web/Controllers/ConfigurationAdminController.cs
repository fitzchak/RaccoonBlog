using System.Web.Mvc;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Controllers
{
    public class ConfigurationAdminController : AdminController
    {
		public ActionResult Index()
		{
			return View(BlogConfig.MapTo<BlogConfigAdminViewModel>());
		}
    }
}