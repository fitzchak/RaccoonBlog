using AutoMapper;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles
{
	public class BlogConfigAdminViewModelMapperProfile : AbstractProfile
	{
		public BlogConfigAdminViewModelMapperProfile()
		{
			Mapper.CreateMap<BlogConfig, BlogConfigAdminViewModel>()
				;
		}
	}
}