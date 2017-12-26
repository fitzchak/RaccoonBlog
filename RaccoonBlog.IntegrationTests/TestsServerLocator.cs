using Raven.TestDriver;

namespace RaccoonBlog.IntegrationTests
{
    public class TestsServerLocator : RavenServerLocator
    {
        public override string ServerPath { get; } = @"C:\RavenDB\Server\Raven.Server.exe";
    }
}