using System.Web;
using RaccoonBlog.Web.Controllers;
using RaccoonBlog.Web.Models;
using Raven.Client;

namespace RaccoonBlog.Web.DataServices
{
    public class UserService
    {
        protected IDocumentSession Session { get; set; }

        public UserService(IDocumentSession session)
        {
            Session = session;
        }

        public User GetCurrentUser()
        {
            if (HttpContext.Current.Request.IsAuthenticated == false)
                return null;

            var email = HttpContext.Current.User.Identity.Name;
            var user = Session.GetUserByEmail(email);
            return user;
        }
    }
}