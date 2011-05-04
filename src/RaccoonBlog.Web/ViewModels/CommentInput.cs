using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RavenDbBlog.ViewModels
{
    public class CommentInput
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Url")]
        public string Url { get; set; }

        [Required]
        [Display(Name = "Comments")]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

        [HiddenInput]
        public string CommenterKey { get; set; }
    }
}