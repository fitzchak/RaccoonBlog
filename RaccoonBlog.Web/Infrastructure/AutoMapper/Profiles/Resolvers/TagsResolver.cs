using System;
using System.Collections.Generic;
using System.Linq;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers
{
	public class TagsResolver
	{
		private const string TagsSeparator = "|";

		public static string ResolveTags(ICollection<String> tags)
		{	
			return string.Join(TagsSeparator, tags);
		}

		public static ICollection<String> ResolveTagsInput(string tags)
		{
			return tags.Split(new[] {TagsSeparator}, StringSplitOptions.RemoveEmptyEntries);
		}
	}
}
