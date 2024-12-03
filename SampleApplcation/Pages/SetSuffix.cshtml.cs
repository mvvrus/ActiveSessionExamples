using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace SampleApplication.Pages
{
    public class SetSuffixModel : PageModel
    {
        [RegularExpression("[A-Za-z0-9]*")]
        [BindProperty]
        public String? Suffix { get; set; }

        public void OnGet()
        {
        }

        public ActionResult OnPost()
        {
            if(ModelState.IsValid) {
                return RedirectToPage("Index", new { Suffix });
            }
            else return Page();
        }
    }
}
