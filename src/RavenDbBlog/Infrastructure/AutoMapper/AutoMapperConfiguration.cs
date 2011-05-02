using System;
using System.Web.Mvc;
using AutoMapper;
using RavenDbBlog.Infrastructure.AutoMapper.Profiles;
using RavenDbBlog.Infrastructure.AutoMapper.Profiles.Resolvers;

namespace RavenDbBlog.Infrastructure.AutoMapper
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.CreateMap<string, MvcHtmlString>().ConvertUsing<MvcHtmlStringConverter>();

            Mapper.CreateMap<DateTimeOffset, DateTime>().ConvertUsing<DateTimeTypeConverter>();

            Mapper.AddProfile(new PostViewModelMapperProfile());
            Mapper.AddProfile(new PostsViewModelMapperProfile());
            Mapper.AddProfile(new TagsListViewModelMapperProfile());
            Mapper.AddProfile(new SectionMapperProfile());

            Mapper.AddProfile(new UserAdminMapperProfile());
            Mapper.AddProfile(new PostsAdminViewModelMapperProfile());
        }
    }
}