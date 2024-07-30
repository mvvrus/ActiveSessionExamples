using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVVrus.AspNetCore.ActiveSession;

namespace EnumSapmleApplication.Pages
{
    public class IndexModel : PageModel
    {
        internal IActiveSession? _activeSession;
        public void OnGet()
        {
            _activeSession=HttpContext.GetActiveSession();
        }
    }
}
