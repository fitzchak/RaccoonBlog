using AutoMapper;
using RavenDbBlog.Infrastructure.AutoMapper;
using Xunit;

namespace IntegrationTests.AutoMapper
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