using AutoMapper;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;
using RaccoonBlog.Web.Infrastructure.Common;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles
{
	public class SyndicationMapperProfile : Profile
	{
		protected override void Configure()
		{
			Mapper.CreateMap<PostComments.Comment, CommentRssFeedViewModel>()
				.ForMember(x => x.CreatedAt, o => o.MapFrom(m => m.CreatedAt.ToString("R")))
				.ForMember(x => x.PostId, o => o.Ignore())
				.ForMember(x => x.CommentId, o => o.MapFrom(x=>x.Id))
				.ForMember(x => x.PostTitle, o => o.Ignore())
				.ForMember(x => x.PostSlug, o => o.Ignore())
				;

			Mapper.CreateMap<Post, CommentRssFeedViewModel>()
				.ForMember(x => x.PostId, o => o.MapFrom(m => RavenIdResolver.Resolve(m.Id)))
				.ForMember(x => x.PostTitle, o => o.MapFrom(m => m.Title))
				.ForMember(x => x.PostSlug, o => o.MapFrom(m => SlugConverter.TitleToSlug(m.Title)))
				.ForMember(x => x.Author, o => o.Ignore())
				.ForMember(x=>x.Body, o=>o.Ignore())
				;

			Mapper.CreateMap<Post, PostRssFeedViewModel>()
				.ForMember(x => x.Id, o => o.MapFrom(m => RavenIdResolver.Resolve(m.Id)))
				.ForMember(x => x.Slug, o => o.MapFrom(m => SlugConverter.TitleToSlug(m.Title)))
				.ForMember(x => x.PublishedAt, o => o.MapFrom(m => m.PublishAt.ToString("R")))
				;
		}
	}
}