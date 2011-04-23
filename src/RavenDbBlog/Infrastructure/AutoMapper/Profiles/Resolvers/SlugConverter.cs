using AutoMapper;

namespace RavenDbBlog.Infrastructure.AutoMapper.Profiles.Resolvers
{
    public class SlugConverter : TypeConverter<string, string>
    {
        protected override string ConvertCore(string source)
        {
            return TitleToSlag(source);
        }

        public static string TitleToSlag(string title)
        {
            return title;
        }
    }
}