using System;
using FluentScheduler;

namespace RaccoonBlog.Web.Infrastructure.Jobs
{
    public class SocialNetworkIntegrationJobsRegistry : Registry
    {
        public SocialNetworkIntegrationJobsRegistry()
        {
            Schedule<RedditIntegration>()
                .WithName("RedditIntegration")
                .NonReentrant()
                .ToRunEvery(5)
                .Minutes();
        }
    }
}