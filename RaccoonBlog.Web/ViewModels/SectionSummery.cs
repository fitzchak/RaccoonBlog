using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RaccoonBlog.Web.ViewModels
{
    public class SectionSummery
    {
        [HiddenInput]
        public int Id { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Active?")]
        public bool IsActive { get; set; }
    }
}
