using System;
using System.Web.Mvc;
using AutoMapper;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.CreateMap<string, MvcHtmlString>().ConvertUsing<MvcHtmlStringConverter>();
            Mapper.CreateMap<Guid, string>().ConvertUsing<GuidToStringConverter>();

            Mapper.CreateMap<DateTimeOffset, DateTime>().ConvertUsing<DateTimeTypeConverter>();

            Mapper.AddProfile(new PostViewModelMapperProfile());
            Mapper.AddProfile(new PostsViewModelMapperProfile());
            Mapper.AddProfile(new TagsListViewModelMapperProfile());
            Mapper.AddProfile(new SectionMapperProfile());

            Mapper.AddProfile(new UserAdminMapperProfile());
            Mapper.AddProfile(new PostsAdminViewModelMapperProfile());
            Mapper.AddProfile(new SectionAdminMapperProfile());
        }
    }
}