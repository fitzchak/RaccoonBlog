using AutoMapper;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using Xunit;

namespace RaccoonBlog.IntegrationTests.AutoMapper
{
	public class AutoMapperConfigurationTester
	{
		[Fact]
        public void AssertConfigurationIsValid()
		{
			AutoMapperConfiguration.Configure();
			Mapper.AssertConfigurationIsValid();
		}
	}
}
