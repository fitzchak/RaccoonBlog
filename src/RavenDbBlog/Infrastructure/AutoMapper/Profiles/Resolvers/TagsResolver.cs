using System;
using System.Linq;

namespace RavenDbBlog.Infrastructure.AutoMapper.Profiles.Resolvers
{
    public class TagsResolver
    {
        private const string TagsSeparator = "||";

        public static string ResolveTags(string[] tags)
        {
            string result = tags.Aggregate(string.Empty, (current, tag) => current + (TagsSeparator + tag));
            return result.Trim();
        }

        public static string[] ResolveTagsInput(string tags)
        {
            return tags.Split(new[] {TagsSeparator}, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}