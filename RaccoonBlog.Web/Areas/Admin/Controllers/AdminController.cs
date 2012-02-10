using System.Web.Mvc;
using RaccoonBlog.Web.Controllers;

namespace RaccoonBlog.Web.Areas.Admin.Controllers
{
	[Authorize]
	public abstract class AdminController : RaccoonController
	{
	}
}