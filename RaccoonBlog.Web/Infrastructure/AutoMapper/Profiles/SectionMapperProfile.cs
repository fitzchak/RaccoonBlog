using System.Web;
using AutoMapper;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;
using RaccoonBlog.Web.Infrastructure.Common;
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

			Mapper.CreateMap<Posts_Statistics.ReduceResult, PostsStatisticsViewModel>()
			   ;

			Mapper.CreateMap<Post, RecentCommentViewModel>()
				.ForMember(x => x.PostId, o => o.MapFrom(m => RavenIdResolver.Resolve(m.Id)))
				.ForMember(x => x.PostTitle, o => o.MapFrom(m => m.Title))
				.ForMember(x => x.PostSlug, o => o.MapFrom(m => SlugConverter.TitleToSlug(m.Title)))
				.ForMember(x => x.Author, o => o.Ignore())
				.ForMember(x => x.CommentId, o => o.Ignore())
				.ForMember(x => x.ShortBody, o => o.Ignore())
				;

			Mapper.CreateMap<PostComments.Comment, RecentCommentViewModel>()
				.ForMember(x => x.PostId, o => o.Ignore())
				.ForMember(x => x.ShortBody, o => o.MapFrom(x => MaxLength(x.Body, 128)))
				.ForMember(x => x.CommentId, o => o.MapFrom(x => x.Id))
				.ForMember(x => x.PostSlug, o => o.Ignore())
				.ForMember(x => x.PostTitle, o => o.Ignore())
				;
		}

		private static string MaxLength(string text, int len)
		{
			if (text == null)
				return null;
			if(text.Length > len)
			{
				return text.Substring(0, len - 3) + "...";
			}
			return text;
		}
	}
}
