using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RavenDbBlog.ViewModels
{
    public class PostInput
    {
        [HiddenInput]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Body")]
        [DataType(DataType.MultilineText)]
        public MvcHtmlString Body { get; set; }

        [Display(Name = "Created At")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset CreatedAt { get; set; }

        [Display(Name = "Publish At")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset PublishAt { get; set; }

        [Display(Name = "Tags")]
        public string Tags { get; set; }

        [Display(Name = "Allow Comments?")]
        public bool AllowComments { get; set; }
    }
}