using System.Web.Mvc;

namespace RaccoonBlog.Web.Controllers
{
    [Authorize]
    public abstract class AdminController : AbstractController
    {
    }
}