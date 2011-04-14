using AutoMapper;
using RavenDbBlog.Infrastructure.AutoMapper;
using Xunit;

namespace CodeCampServer.IntegrationTests.UI
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