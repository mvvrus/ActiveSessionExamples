using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SampleApplication.Pages
{
    public class TimeSeriesParamsModel : PageModel
    {
        [DisplayName("Interval between measurements(sec)")]
        [Required]
        [Range(1,3600)]
        [BindProperty]
        public Int32 Interval { get; set; } = 10;

        public void OnGet()
        {
        }

        public ActionResult OnPost() 
        {
            return Page();
        }
    }
}
