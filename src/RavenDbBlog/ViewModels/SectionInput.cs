using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using RavenDbBlog.Helpers.Validation;

namespace RavenDbBlog.ViewModels
{
    public class SectionInput
    {
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [AllowHtml]
        [Display(Name = "Body")]
        public string Body { get; set; }

        [RequiredIf("Body", "")]
        [Display(Name = "Controller Name")]
        public string ControllerName { get; set; }

        [RequiredIf("Body", "")]
        [Display(Name = "Action Name")]
        public string ActionName { get; set; }

        public bool IsActionSection()
        {
            return string.IsNullOrEmpty(Body);
        }
    }
}