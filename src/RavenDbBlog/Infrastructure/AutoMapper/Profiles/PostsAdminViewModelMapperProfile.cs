using System;
using System.Web.Mvc;
using AutoMapper;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Infrastructure.AutoMapper.Profiles.Resolvers;
using RavenDbBlog.ViewModels;

namespace RavenDbBlog.Infrastructure.AutoMapper.Profiles
{
    public class PostsAdminViewModelMapperProfile : AbstractProfile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Post, PostSummaryJson>()
                .ForMember(x => x.Id, o => o.MapFrom(m => RavenIdResolver.Resolve(m.Id)))
                .ForMember(x => x.Title, o => o.MapFrom(m => MvcHtmlString.Create(m.Title)))
                .ForMember(x => x.Start, o => o.MapFrom(m => m.PublishAt.ToString("yyyy-MM-ddTHH:mm:ssZ")))
                .ForMember(x => x.Url, o => o.MapFrom(m => UrlHelper.Action("Details", "PostAdmin", new { Id = RavenIdResolver.Resolve(m.Id), Slug = SlugConverter.TitleToSlag(m.Title) })))
                ;
        }
    }
}