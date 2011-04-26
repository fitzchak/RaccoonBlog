using System;
using AutoMapper;
using RavenDbBlog.Infrastructure.AutoMapper.Profiles;
using RavenDbBlog.Infrastructure.AutoMapper.Profiles.Resolvers;

namespace RavenDbBlog.Infrastructure.AutoMapper
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.CreateMap<DateTimeOffset, DateTime>().ConvertUsing<DateTimeTypeConverter>();

            Mapper.AddProfile(new PostViewModelMapperProfile());
            Mapper.AddProfile(new PostsViewModelMapperProfile());

            Mapper.AddProfile(new UserAdminMapperProfile());
            Mapper.AddProfile(new PostsAdminViewModelMapperProfile());
        }
    }
}