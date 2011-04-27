using System.Web;
using System.Web.Mvc;
using AutoMapper;

namespace RavenDbBlog.Infrastructure.AutoMapper.Profiles
{
    public abstract class AbstractProfile : Profile
    {
        private UrlHelper _urlHelper;
        protected UrlHelper UrlHelper
        {
            get
            {
                return _urlHelper ?? (_urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext));
            }
        }
    }
}