using AutoMapper;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles
{
	public class BlogConfigAdminViewModelMapperProfile : AbstractProfile
	{
		public BlogConfigAdminViewModelMapperProfile()
		{
			Mapper.CreateMap<BlogConfig, BlogConfigurationInput>()
				;

			Mapper.CreateMap<BlogConfigurationInput, BlogConfig>()
				.ForMember(x => x.Id, o => o.Ignore())
				;
		}
	}
}