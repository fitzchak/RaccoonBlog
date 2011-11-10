// //-----------------------------------------------------------------------
// // <copyright company="Hibernating Rhinos LTD">
// //     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// // </copyright>
// //-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using RaccoonBlog.Web.Infrastructure.Common;

namespace RaccoonBlog.Web.Models
{
	public class Post
	{
		public string Id { get; set; }

		public string Title { get; set; }
		public string LegacySlug { get; set; }

		public string Body { get; set; }
		public ICollection<string> Tags { get; set; }

		public string AuthorId { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset PublishAt { get; set; }
		public bool SkipAutoReschedule { get; set; }

		public string LastEditedByUserId { get; set; }
		public DateTimeOffset? LastEditedAt { get; set; }

		public bool IsDeleted { get; set; }
		public bool AllowComments { get; set; }

		private Guid _showPostEvenIfPrivate;
		public Guid ShowPostEvenIfPrivate
		{
			get
			{
				if (_showPostEvenIfPrivate == Guid.Empty)
					_showPostEvenIfPrivate = Guid.NewGuid();
				return _showPostEvenIfPrivate;
			}
			set { _showPostEvenIfPrivate = value; }
		}

		public int CommentsCount { get; set; }

		public string CommentsId { get; set; }

		public IEnumerable<string> TagsAsSlugs
		{
			get
			{
				if (Tags == null)
					yield break;
				foreach (var tag in Tags)
				{
					yield return SlugConverter.TitleToSlug(tag);
				}
			}
		}

        public bool IsPublicPost(Guid key)
        {
			if (IsDeleted)
				return false;

            if (PublishAt <= DateTimeOffset.Now)
                return true;

			return key != Guid.Empty && key == ShowPostEvenIfPrivate;
		}
	}
}
