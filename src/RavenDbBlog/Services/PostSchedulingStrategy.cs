using System;
using Raven.Client;

namespace RavenDbBlog.Services
{
	public class PostSchedulingStrategy
	{
		private IDocumentSession session;
		private readonly DateTimeOffset? requestedDate;
		private readonly DateTimeOffset? currentDate;

		public PostSchedulingStrategy(IDocumentSession session, DateTimeOffset? requestedDate, DateTimeOffset? currentDate)
		{
			this.session = session;
			this.requestedDate = requestedDate;
			this.currentDate = currentDate;
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
		public DateTimeOffset Schedule()
		{
			throw new NotImplementedException();
		}
	}
}