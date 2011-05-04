using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using RavenDbBlog.Helpers.Validation;

namespace RavenDbBlog.ViewModels
{
    public class SectionInput
    {
        [HiddenInput]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [AllowHtml]
        [Display(Name = "Body")]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

        [RequiredIf("Body", "")]
        [Display(Name = "Controller Name")]
        public string ControllerName { get; set; }

        [RequiredIf("Body", "")]
        [Display(Name = "Action Name")]
        public string ActionName { get; set; }

        [Display(Name = "Active?")]
        public bool IsActive { get; set; }

        public bool IsNewSection()
        {
            return Id == 0;
        }
    }
}