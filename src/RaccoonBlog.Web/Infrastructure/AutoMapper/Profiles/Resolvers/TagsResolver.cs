using System;
using System.Linq;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers
{
    public class TagsResolver
    {
        private const string TagsSeparator = "|";

        public static string ResolveTags(string[] tags)
        {	
        	return string.Join(TagsSeparator, tags);
        }

        public static string[] ResolveTagsInput(string tags)
        {
            return tags.Split(new[] {TagsSeparator}, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
