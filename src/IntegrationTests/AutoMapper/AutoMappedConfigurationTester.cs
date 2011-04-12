using AutoMapper;
using RavenDbBlog.AutoMapper;
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