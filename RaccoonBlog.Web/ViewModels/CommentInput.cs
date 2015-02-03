using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DataAnnotationsExtensions;

namespace RaccoonBlog.Web.ViewModels
{
	public class CommentInput
	{
        [Required]
		[Display(Name = "Name")]
		public string Name { get; set; }

        [Required]
		[Display(Name = "Email")]
        [Email]
		public string Email { get; set; }

		[Display(Name = "Url")]
		public string Url { get; set; }

		[AllowHtml]
        [Required]
		[Display(Name = "Comments")]
		[DataType(DataType.MultilineText)]
		public string Body { get; set; }

		[HiddenInput]
		public Guid? CommenterKey { get; set; }
	}
}
