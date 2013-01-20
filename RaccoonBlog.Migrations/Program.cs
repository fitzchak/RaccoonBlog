using System;
using System.Linq;
using RaccoonBlog.Web.Models;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Linq;

namespace RaccoonBlog.Migrations
{
	class Program
	{
		static void Main()
		{
			using (var store = new DocumentStore { ConnectionStringName = "RavenDB" }.Initialize())
			{
				int start = 0;
				while (true)
				{
					using (var session = store.OpenSession())
					{
						var posts = session.Query<Post>()
							.OrderBy(x => x.CreatedAt)
							.Include(x => x.CommentsId)
							.Skip(start)
							.Take(128)
							.ToList();

						if (posts.Count == 0)
							break;

						foreach (var post in posts)
						{
							session.Load<PostComments>(post.CommentsId).Post = new PostComments.PostReference
							{
								Id = post.Id,
								PublishAt = post.PublishAt
							};
						}

						session.SaveChanges();
						start += posts.Count;
						Console.WriteLine("Migrated {0}", start);
					}
				}
			}
		}
	}
}
