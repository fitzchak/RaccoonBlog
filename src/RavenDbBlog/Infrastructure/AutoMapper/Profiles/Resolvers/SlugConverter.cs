using AutoMapper;

namespace RavenDbBlog.Infrastructure.AutoMapper.Profiles.Resolvers
{
    public class SlugConverter
    {
        public static string TitleToSlag(string title)
        {
            return title;
        }
    }
}