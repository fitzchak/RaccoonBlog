using System;
using System.Linq;
using RaccoonBlog.Web.Models;
using Raven.Client;

namespace RaccoonBlog.Web.Services
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

	    private DateTimeOffset GetLastPostOnSchedule()
	    {
	    	var p = session.Query<Post>()
	    		.OrderByDescending(post => post.PublishAt)
	    		.Select(post => new {post.PublishAt})
	    		.FirstOrDefault();

	        return p != null ? p.PublishAt : DateTimeOffset.Now;
	    }
	}
}