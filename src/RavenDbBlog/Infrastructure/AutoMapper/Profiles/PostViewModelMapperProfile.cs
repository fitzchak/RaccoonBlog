using System.Configuration;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using RavenDbBlog.Core;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Infrastructure.AutoMapper.Profiles.Resolvers;
using RavenDbBlog.ViewModels;

namespace RavenDbBlog.Infrastructure.AutoMapper.Profiles
{
    public class PostViewModelMapperProfile : AbstractProfile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Post, PostViewModel.PostDetails>()
                .ForMember(x => x.Id, o => o.MapFrom(m => RavenIdResolver.Resolve(m.Id)))
                .ForMember(x => x.Slug, o => o.MapFrom(m => SlugConverter.TitleToSlag(m.Title)))
                .ForMember(x => x.PublishedAt, o => o.MapFrom(m => m.PublishAt))
                ;

            Mapper.CreateMap<PostComments.Comment, PostViewModel.Comment>()
                .ForMember(x => x.Body, o => o.MapFrom(m => MarkdownResolver.Resolve(m.Body)))
                .ForMember(x => x.EmailHash, o => o.MapFrom(m => EmailHashResolver.Resolve(m.Email)))
                .ForMember(x => x.IsImportant, o => o.MapFrom(m => m.Important))
                ;

            Mapper.CreateMap<Post, PostReference>();
            
            Mapper.CreateMap<Commenter, CommentInput>()
                .ForMember(x => x.Body, o => o.Ignore())
                .ForMember(x => x.CommenterKey, o => o.MapFrom(m => m.Key))
                ;

            Mapper.CreateMap<User, CommentInput>()
                .ForMember(x => x.Name, o => o.MapFrom(m => m.FullName))
                .ForMember(x => x.Url, o => o.MapFrom(m => ConfigurationManager.AppSettings["MainUrl"] + UrlHelper.RouteUrl("Default")))
                .ForMember(x => x.Body, o => o.Ignore())
                .ForMember(x => x.CommenterKey, o => o.Ignore())
                ;

            Mapper.CreateMap<HttpRequestWrapper, RequestValues>();
        }
    }
}