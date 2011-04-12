using System;
using System.Web.Mvc;
using AutoMapper;
using RavenDbBlog.Core.Models;
using RavenDbBlog.ViewModels;

namespace RavenDbBlog.AutoMapper
{
    public class PostsViewModelMapperProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Post, PostsViewModel.PostSummary>()
                .ForMember(x => x.Id, o => o.MapFrom(m => RavenIdResolver.Resolve(m.Id)))
                .ForMember(x => x.PublishedAt, o => o.MapFrom(m => m.PublishAt))
                .ForMember(x => x.Title, o => o.MapFrom(m => MvcHtmlString.Create(m.Title)))
                .ForMember(x => x.Body, o => o.MapFrom(m => MvcHtmlString.Create(m.Body)));
        }
    }
}