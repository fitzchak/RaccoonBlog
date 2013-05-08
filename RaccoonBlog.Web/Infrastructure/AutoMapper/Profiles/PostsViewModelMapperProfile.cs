using System.Web;
using AutoMapper;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;
using RaccoonBlog.Web.Infrastructure.Common;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles
{
	public class PostsViewModelMapperProfile : Profile
	{
		protected override void Configure()
		{
			Mapper.CreateMap<Post, PostsViewModel.PostSummary>()
				.ForMember(x => x.Id, o => o.MapFrom(m => RavenIdResolver.Resolve(m.Id)))
				.ForMember(x => x.Slug, o => o.MapFrom(m => SlugConverter.TitleToSlug(m.Title)))
				.ForMember(x => x.Author, o => o.Ignore())
				.ForMember(x => x.PublishedAt, o => o.MapFrom(m => m.PublishAt))
				.ForMember(x=>x.Title, o => o.MapFrom(m => HttpUtility.HtmlDecode(m.Title)))
				;

			Mapper.CreateMap<User, PostsViewModel.PostSummary.UserDetails>();

			Mapper.CreateMap<string, TagDetails>()
				.ForMember(x => x.Name, o => o.MapFrom(m => m))
				;
		}
	}
}
