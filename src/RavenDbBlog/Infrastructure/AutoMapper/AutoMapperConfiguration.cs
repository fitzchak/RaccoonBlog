using AutoMapper;
using RavenDbBlog.Infrastructure.AutoMapper.Profiles;

namespace RavenDbBlog.Infrastructure.AutoMapper
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.AddProfile(new PostViewModelMapperProfile());
            Mapper.AddProfile(new PostsViewModelMapperProfile());
            Mapper.AddProfile(new UserAdminMapperProfile());
        }
    }
}