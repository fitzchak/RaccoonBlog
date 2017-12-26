using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace RaccoonBlog.Web.ViewModels
{
	public class CommentInput
	{
		[Required(ErrorMessage = "Name is required")]
		[Display(Name = "Name")]
		public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
		[Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
		public string Email { get; set; }

		[Display(Name = "Url")]
		public string Url { get; set; }

		[Required(ErrorMessage = "Comment is required")]
		[Display(Name = "Comments")]
		[DataType(DataType.MultilineText)]
		public string Body { get; set; }

		[HiddenInput]
		public Guid? CommenterKey { get; set; }
	}
}
