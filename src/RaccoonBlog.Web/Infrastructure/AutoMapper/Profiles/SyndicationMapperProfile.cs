using AutoMapper;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles
{
	public class SyndicationMapperProfile : Profile
	{
		protected override void Configure()
		{
			Mapper.CreateMap<PostComments.Comment, CommentRssFeedViewModel>()
				.ForMember(x => x.Post, o => o.Ignore())
				;

			Mapper.CreateMap<Post, CommentRssFeedViewModel.PostSummary>()
				.ForMember(x => x.Id, o => o.MapFrom(m => RavenIdResolver.Resolve(m.Id)))
				;
		}
	}
}