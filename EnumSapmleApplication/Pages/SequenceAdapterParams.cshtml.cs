using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SapmleApplication.Models;

namespace SapmleApplication.Pages
{
    public class SequenceAdapterParamsModel : PageModel
    {
        [BindProperty]
        public SequenceParams? Input { get; set; }
        public void OnGet()
        {
        }

        public void OnPost() 
        {
        }
    }
}
