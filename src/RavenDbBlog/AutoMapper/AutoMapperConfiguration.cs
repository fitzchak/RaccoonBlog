using AutoMapper;

namespace RavenDbBlog.AutoMapper
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.AddProfile(new PostViewModelMapperProfile());
            Mapper.AddProfile(new PostsViewModelMapperProfile());
        }
    }
}