using System;
using System.Linq;
using RaccoonBlog.Web.Infrastructure.Common;
using RaccoonBlog.Web.Models;
using Raven.Client;

namespace RaccoonBlog.Web.Services
{
	public class PostSchedulingStrategy
	{
		private readonly IDocumentSession session;
		private readonly DateTimeOffset now;

		public PostSchedulingStrategy(IDocumentSession session, DateTimeOffset now)
		{
			this.session = session;
			this.now = now;
		}

		/// <summary>
		/// The rules are simple:
		/// * If there is no set date, schedule in at the end of the queue, but on a Monday - Friday 
		/// * If there is a set date, move all the posts from that day one day forward
		///  * only to Monday - Friday
		///  * don't touch posts that are marked with SkipAutoReschedule = true
		/// * If we are moving a current post, we need to:
		///  * trim all the holes in the schedule
		/// </summary>
        public DateTimeOffset Schedule(DateTimeOffset? requestedDate = null)
		{
            if (requestedDate == null)
            {
            	var lastScheduledPostDate = GetLastScheduledPostDate();
				if (lastScheduledPostDate == null || lastScheduledPostDate <= now)
					return now
						.SkipToNextWorkDay()
						.AtNoon();


            	return lastScheduledPostDate.Value
            		.AddDays(1)
					.SkipToNextWorkDay()
            		.AtNoon();
            }

			var postsQuery = from p in session.Query<Post>()
	                         where p.PublishAt > requestedDate && p.SkipAutoReschedule == false && p.PublishAt > now
	                         orderby p.PublishAt
	                         select p;

	    	var nextPostDate = requestedDate.Value;
	        foreach (var post in postsQuery)
	        {
				post.PublishAt
					= nextPostDate 
					= nextPostDate
						.AddDays(1)
						.SkipToNextWorkDay()
						.AtTime(post.PublishAt);
	        }

	    	return requestedDate.Value;
		}

	    private DateTimeOffset? GetLastScheduledPostDate()
	    {
	    	var p = session.Query<Post>()
	    		.OrderByDescending(post => post.PublishAt)
	    		.Select(post => new {post.PublishAt})
	    		.FirstOrDefault();

	    	return p != null ? p.PublishAt : (DateTimeOffset?)null;
	    }
	}
}
