using System.Web.Mvc;
using AutoMapper;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Infrastructure.AutoMapper.Profiles.Resolvers;
using RavenDbBlog.ViewModels;

namespace RavenDbBlog.Infrastructure.AutoMapper.Profiles
{
    public class PostViewModelMapperProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Post, PostViewModel.PostDetails>()
                .ForMember(x => x.Id, o => o.MapFrom(m => RavenIdResolver.Resolve(m.Id)))
                .ForMember(x => x.PublishedAt, o => o.MapFrom(m => m.PublishAt))
                .ForMember(x => x.Title, o => o.MapFrom(m => MvcHtmlString.Create(m.Title)))
                .ForMember(x => x.Body, o => o.MapFrom(m => MvcHtmlString.Create(m.Body)));

            Mapper.CreateMap<PostComments.Comment, PostViewModel.Comment>()
                .ForMember(x => x.Body, o => o.MapFrom(m => MarkdownResolver.Resolve(m.Body)))
                .ForMember(x => x.EmailHash, o => o.MapFrom(m => EmailHashResolver.Resolve(m.Email)))
                ;

            Mapper.CreateMap<CommentInput, Commenter>();

            Mapper.CreateMap<Commenter, CommentInput>();
        }
    }
}