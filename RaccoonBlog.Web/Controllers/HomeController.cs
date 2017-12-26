using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RaccoonBlog.Web.Models;

namespace RaccoonBlog.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            throw new NotSupportedException();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
