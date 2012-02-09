using AutoMapper;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
using Xunit;

namespace RaccoonBlog.IntegrationTests.AutoMapper
{
	public class AutoMapperConfigurationTester
	{
		public AutoMapperConfigurationTester()
		{
			AutoMapperConfiguration.Configure();
		}

		[Fact]
		public void AssertConfigurationIsValid()
		{
			Mapper.AssertConfigurationIsValid();
		}

		[Fact]
		public void CanMapFromCommentToNewCommentViewModel()
		{
			var comment = new PostComments.Comment();
			Assert.DoesNotThrow(() => comment.MapTo<NewCommentEmailViewModel>());
		}
	}
}