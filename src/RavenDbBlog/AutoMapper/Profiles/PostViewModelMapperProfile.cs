using System.Web.Mvc;
using AutoMapper;
using RavenDbBlog.Core.Models;
using RavenDbBlog.ViewModels;

namespace RavenDbBlog.AutoMapper
{
    public class PostViewModelMapperProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Post, PostViewModel.PostDetails>()
                .ForMember(x => x.Id, o => o.MapFrom(m => RavenIdResolver.Resolve(m.Id)))
                .ForMember(x => x.PublishedAt, o => o.MapFrom(m => m.PublishAt))
                .ForMember(x => x.Body, o => o.MapFrom(m => MvcHtmlString.Create(m.Body)));

            Mapper.CreateMap<CommentsCollection.Comment, PostViewModel.Comment>()
                .ForMember(x => x.Body, o => o.MapFrom(m => MvcHtmlString.Create(m.Body)));
        }
    }
}