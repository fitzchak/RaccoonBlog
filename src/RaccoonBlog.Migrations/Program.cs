using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using RaccoonBlog.Web.Models;
using Raven.Client.Document;

namespace RaccoonBlog.Migrations
{
	class Program
	{
		static void Main()
		{
			using(var store = new DocumentStore{Url = "http://localhost:9191"}.Initialize())
			{
				int start = 0;
				while (true)
				{
					using(var session = store.OpenSession())
					{
						var posts = session.Query<Post>()
							.Skip(start)
							.Take(128)
							.ToList();

						if (posts.Count == 0)
							break;

						foreach (var post in posts)
						{
							post.Title = HttpUtility.HtmlDecode(post.Title);
						}

						session.SaveChanges();
					}

					start += 128;
				}
			}
		}
	}
}
