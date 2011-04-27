using System.Web.Mvc;

namespace RavenDbBlog.Controllers
{
    [Authorize]
    public abstract class AdminController : AbstractController
    {
    }
}