using System.IO;
using System.Text;
using System.Web.Mvc;
using dotless.Core.configuration;

namespace RaccoonBlog.Web.Controllers
{
    public class CssController : Controller
    {
         public ActionResult Merge(string[] files)
         {
             var builder = new StringBuilder();
             foreach (var file in files)
             {
                 var pathAllowed = Server.MapPath(Url.Content("~/Content/css"));
                 var normalizeFile = Server.MapPath(Url.Content(Path.Combine("~/Content/css", file)));
                 if (normalizeFile.StartsWith(pathAllowed) == false)
                 {
                     return HttpNotFound("Path not allowed");
                 }
                 if (System.IO.File.Exists(normalizeFile))
                 {
                     builder.AppendLine(System.IO.File.ReadAllText(normalizeFile));
                 }
             }

             // take care of etags!

             var css = dotless.Core.Less.Parse(builder.ToString(), new DotlessConfiguration
                                                             {

                                                             });

             return Content(css, "text/css");
         }
    }
}