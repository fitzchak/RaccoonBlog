namespace RaccoonBlog.Web.ViewModels
{
	public class PostRssFeedViewModel
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Slug { get; set; }
		public string Body { get; set; }
		public string PublishedAt { get; set; }
	}
}