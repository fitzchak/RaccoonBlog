using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DataAnnotationsExtensions;

namespace RaccoonBlog.Web.ViewModels
{
	public class CommentInput
	{
		[Required(ErrorMessage = "Name is required")]
		[Display(Name = "Name")]
		public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
		[Display(Name = "Email")]
        [Email(ErrorMessage = "Email is invalid")]
		public string Email { get; set; }

		[Display(Name = "Url")]
		public string Url { get; set; }

		[AllowHtml]
		[Required(ErrorMessage = "Comment is required")]
		[Display(Name = "Comments")]
		[DataType(DataType.MultilineText)]
		public string Body { get; set; }

		[HiddenInput]
		public Guid? CommenterKey { get; set; }
	}
}
