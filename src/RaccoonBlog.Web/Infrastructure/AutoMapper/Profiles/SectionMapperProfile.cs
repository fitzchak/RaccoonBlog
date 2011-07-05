using System.Web;
using AutoMapper;
using RaccoonBlog.Web.Infrastructure.Indexes;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles
{
	public class SectionMapperProfile : Profile
	{
		protected override void Configure()
		{
			Mapper.CreateMap<Section, SectionDetails>()
				;

			Mapper.CreateMap<Post, FuturePostViewModel>()
				.ForMember(x => x.Title, o => o.MapFrom(m => HttpUtility.HtmlDecode(m.Title)))
			   ;

			Mapper.CreateMap<PostsStatistics, PostsStatisticsViewModel>()
			   ;
		}
	}
}