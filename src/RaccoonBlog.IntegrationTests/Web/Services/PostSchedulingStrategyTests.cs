using System;
using System.Collections.Generic;
using RaccoonBlog.Web.Infrastructure.Common;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.Services;
using Raven.Client;
using Raven.Client.Embedded;
using Xunit;

namespace RaccoonBlog.IntegrationTests.Web.Services
{
	public class PostSchedulingStrategyTests
	{
		protected DateTimeOffset Now { get; private set; }
		protected IDocumentSession Session { get; private set; }

		public PostSchedulingStrategyTests()
		{
			Now = DateTimeOffset.Now;

			var documentStore = new EmbeddableDocumentStore { RunInMemory = true }.Initialize();
			Session = documentStore.OpenSession();
		}

		~PostSchedulingStrategyTests()
		{
			Session.Dispose();
		}

		[Fact]
		public void WhenPostingNewPostWithoutPublishDateSpecified_AndTheLastPostPublishDateIsAFewDaysAgo_ScheduleItForToday()
		{
			Session.Store(new Post { PublishAt = Now.AddDays(-3) });
			var rescheduler = new PostSchedulingStrategy(Session, Now);

			var scheduleDate = Now.AddDays(1).AtNoon();
			var scheduled = rescheduler.Schedule();
			Assert.Equal(scheduleDate, scheduled);
		}

		[Fact]
		public void WhenPostingNewPostWithPublishDateSpecified_AndTheLastPostPublishDateIsAFewDaysAgo_ScheduleItForSpecifiedDate()
		{
			Session.Store(new Post { PublishAt = Now.AddDays(-3) });
			var rescheduler = new PostSchedulingStrategy(Session, Now);

			var scheduleDate = Now.AddHours(1);
			Assert.Equal(scheduleDate, rescheduler.Schedule(scheduleDate));
			Assert.NotEqual(scheduleDate.AddDays(-2), rescheduler.Schedule(scheduleDate));
		}
	}
}