using System;
using System.Linq;
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
		protected IDocumentStore DocumentStore { get; private set; }
		protected IDocumentSession Session { get; private set; }

		public PostSchedulingStrategyTests()
		{
			Now = DateTimeOffset.Now;

			DocumentStore = new EmbeddableDocumentStore { RunInMemory = true }.Initialize();
			Session = DocumentStore.OpenSession();
		}

		~PostSchedulingStrategyTests()
		{
			Session.Dispose();
			DocumentStore.Dispose();
		}

		[Fact]
		public void WhenPostingNewPostWithoutPublishDateSpecified_AndTheLastPostPublishDateIsAFewDaysAgo_ScheduleItForToday()
		{
			Session.Store(new Post { PublishAt = Now.AddDays(-3) });
			Assert.Equal(1, Session.Query<Post>().Count());

			var rescheduler = new PostSchedulingStrategy(Session, Now);

			var scheduleDate = Now.AddDays(1).AtNoon();
			var scheduled = rescheduler.Schedule();
			Assert.Equal(scheduleDate, scheduled);
		}

		[Fact]
		public void WhenPostingNewPostWithoutPublishDateSpecified_AndThereIsNoLastPost_ScheduleItForToday()
		{
			var rescheduler = new PostSchedulingStrategy(Session, Now);

			var scheduleDate = Now.AddDays(1).AtNoon();
			var scheduled = rescheduler.Schedule();
			Assert.Equal(scheduleDate, scheduled);
		}

		[Fact]
		public void WhenPostingNewPostWithPublishDateSpecified_AndTheLastPostPublishDateIsAFewDaysAgo_ScheduleItForSpecifiedDate()
		{
			Session.Store(new Post { PublishAt = Now.AddDays(-3) });
			Assert.Equal(1, Session.Query<Post>().Count());

			var rescheduler = new PostSchedulingStrategy(Session, Now);

			var scheduleDate = Now.AddHours(1);
			Assert.Equal(scheduleDate, rescheduler.Schedule(scheduleDate));
			Assert.NotEqual(scheduleDate.AddDays(-2), rescheduler.Schedule(scheduleDate));
		}

		[Fact]
		public void WhenPostingNewPostWithPublishDateSpecified_AndThereIsNoLastPost_ScheduleItForSpecifiedDate()
		{
			var rescheduler = new PostSchedulingStrategy(Session, Now);

			var scheduleDate = Now.AddHours(1);
			Assert.Equal(scheduleDate, rescheduler.Schedule(scheduleDate));
			Assert.NotEqual(scheduleDate.AddDays(-2), rescheduler.Schedule(scheduleDate));
		}

		[Fact]
		public void WhenPostingNewPost_DoNotReschedulePublishedPosts()
		{
			Session.Store(new Post { PublishAt = Now.AddDays(-3) });
			Session.Store(new Post { PublishAt = Now.AddHours(-1) });
			Assert.Equal(2, Session.Query<Post>().Count());

			var rescheduler = new PostSchedulingStrategy(Session, Now);
			rescheduler.Schedule();

			Assert.False(Session.Query<Post>().Any(post => post.PublishAt < Now));
		}
	}
}