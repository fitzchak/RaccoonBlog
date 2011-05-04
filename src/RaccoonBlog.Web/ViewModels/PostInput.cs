using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RaccoonBlog.Web.ViewModels
{
    public class PostInput
    {
        [HiddenInput]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [AllowHtml]
        [Required]
        [Display(Name = "Body")]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

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