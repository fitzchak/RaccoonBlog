using System;
using System.ComponentModel.DataAnnotations;

namespace RaccoonBlog.Web.Areas.Admin.Models
{
    public class FutureRssAccess
    {
        [Required]
        [Display(Name = "User Name", Description = "Name of the user whom should get access")]
        public string User { get; set; }

        [Display(Name = "Token", Description = "The access token")]
        public string Token { get; set; }

        [Display(Name = "Expired On", Description = "When should the access token expired?")]
        [DataType(DataType.Date)]
        public DateTime ExpiredOn { get; set; }

        [Display(Name = "Number of Future Days", Description = "How much future days should the user get access to?")]
        public int NumberOfFutureDays { get; set; }

        public static FutureRssAccess Default => new FutureRssAccess()
        {
            NumberOfFutureDays = 180,
            ExpiredOn = DateTime.UtcNow.AddYears(1)
        };
    }
}