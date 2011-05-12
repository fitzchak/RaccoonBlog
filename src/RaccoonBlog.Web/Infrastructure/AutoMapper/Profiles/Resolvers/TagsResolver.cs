using System;
using System.Linq;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers
{
    public class TagsResolver
    {
        private const char TagsSeparator = '|';

        public static string ResolveTags(string[] tags)
        {
            string result = tags.Aggregate(string.Empty, (current, tag) => current + (TagsSeparator + tag));
            return result.Trim(TagsSeparator);
        }

        public static string[] ResolveTagsInput(string tags)
        {
            return tags.Split(new[] {TagsSeparator}, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
