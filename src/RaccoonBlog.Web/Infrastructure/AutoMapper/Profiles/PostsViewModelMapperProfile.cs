using System.Linq;
using AutoMapper;
using RaccoonBlog.Web.Common;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;
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
                .ForMember(x => x.PublishedAt, o => o.MapFrom(m => m.PublishAt))
                .ForMember(x => x.Tags, o => o.MapFrom(m => m.Tags.Select(name => new TagDetails { Name = name })))
                ;

			Mapper.CreateMap<User, PostsViewModel.PostSummary.UserDetails>();
    
        }
    }
}
