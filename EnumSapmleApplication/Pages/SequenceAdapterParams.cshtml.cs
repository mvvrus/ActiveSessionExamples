using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SapmleApplication.Models;

namespace SapmleApplication.Pages
{
    public class SequenceAdapterParamsModel : PageModel
    {
        [BindProperty]
        public BindParams? Input { get; set; }
        public readonly List<SelectListItem> Units = new List<SelectListItem>
        {
            new SelectListItem("sec","1000",true),
            new SelectListItem("msec","1")
        };

        public void OnGet(String mode)
        {
        }

        public void OnPost(String mode) 
        {
        }

        public class BindParams
        {
            public List<BindStage>? Stages { get; set; }
        }

        public class BindStage
        {
            public Int32 Count { get; set; }
            public Int32 Delay { get; set; }
            public Int32 Scale { get; set; }
        }

    }
}
