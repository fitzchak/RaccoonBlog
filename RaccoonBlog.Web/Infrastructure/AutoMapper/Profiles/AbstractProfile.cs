using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles
{
	public abstract class AbstractProfile : Profile
	{
		//protected UrlHelper UrlHelper => new UrlHelper(HttpContext.Current.Request.RequestContext);
	}
}