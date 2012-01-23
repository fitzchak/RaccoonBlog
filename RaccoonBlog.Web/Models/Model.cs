using System.Web.Mvc;

namespace RaccoonBlog.Web.Models
{
	public class Model
	{
		[HiddenInput]
		public string Id { get; set; }
	}
}