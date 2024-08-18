using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVVrus.AspNetCore.ActiveSession;

namespace EnumSapmleApplication.Pages
{
    public class IndexModel : PageModel
    {
        internal String? _terminateSessionEndpoint;
        internal IActiveSession? _activeSession;
        public void OnGet()
        {
            _terminateSessionEndpoint=Url.ActionLink("TerminateSession", "Sample");
            _activeSession=HttpContext.GetActiveSession();
        }
    }
}
