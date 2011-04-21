using System;
using System.Globalization;
using System.Linq;
using Raven.Client;
using RavenDbBlog.Core.Models;

namespace RavenDbBlog.Services
{
	public class PostSchedulingStrategy
	{
		private readonly IDocumentSession session;

	    public PostSchedulingStrategy(IDocumentSession session)
        {
            this.session = session;
        }

	    /// <summary>
		/// The rules are simple:
		/// * If there is no set date, schedule in at the end of the queue, but on a Monday - Friday 
		/// * If there is a set date, move all the posts from that day one day forward
		///  * only to Monday - Friday
		///  * don't touch posts that are marked with SkipAutoReschedule = true
		/// * If we are moving a current post, we need to:
		///  * Check if there is a post in the day we shifted the edited post to, if so, switch them
		///  * Else, trim all the holes in the schedule
		/// </summary>
        public DateTimeOffset Schedule(DateTimeOffset? requestedDate = null)
		{
            if (requestedDate == null)
            {
            	return GetLastPostOnSchedule()
            		.AddDays(1)
					.SkipToNextWorkDay()
            		.AtNoon();
            }

	    	var postsQuery = from p in session.Query<Post>()
	                         where p.PublishAt > requestedDate && p.SkipAutoReschedule == false
	                         orderby p.PublishAt
	                         select p;

	        foreach (var post in postsQuery)
	        {
	        	post.PublishAt = post
					.PublishAt
					.AddDays(1)
					.SkipToNextWorkDay();
	        }

	        return DateTimeOffset.Now;
		}

	    private DateTimeOffset GetLastPostOnSchedule()
	    {
	    	var p = session.Query<Post>()
	    		.OrderByDescending(post => post.PublishAt)
	    		.Select(post => new {post.PublishAt})
	    		.FirstOrDefault();

	        return p != null ? p.PublishAt : DateTimeOffset.Now;
	    }
	}

	public static class DateTimeOffsetExtensions
	{
		public static DateTimeOffset AtNoon(this DateTimeOffset date)
		{
			return new DateTimeOffset(date.Year, date.Month, date.Day, 12, 0, 0, 0, date.Offset);
		}

		public static DateTimeOffset SkipToNextWorkDay(this DateTimeOffset date)
		{
			// we explicitly choose not to user the CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek
			// because we want it to be fixed for what we need, not whatever the user for this is set to.
			if (date.DayOfWeek == DayOfWeek.Saturday)
				return date.AddDays(2);

			if (date.DayOfWeek == DayOfWeek.Sunday)
				return date.AddDays(1);

			return date;
		}
	}
}