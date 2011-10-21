using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaccoonBlog.Web.Infrastructure.Common;
using Xunit;

namespace RaccoonBlog.IntegrationTests.ControllersCore
{
	public class SlugGenerationTests
	{
		[Fact]
		public void IgnoresEntitiesCorrectly()
		{
			var result = SlugConverter.TitleToSlug("Document based modeling: Auctions & Bids");
			Assert.Equal("document-based-modeling-auctions-bids", result);

			result = SlugConverter.TitleToSlug("Hiring Questions–The phone book");
			Assert.Equal("hiring-questions-the-phone-book", result);
		}
	}
}
