using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVVrus.AspNetCore.ActiveSession;

namespace EnumSapmleApplication.Pages
{
    public class IndexModel : PageModel
    {
        internal String? _terminateSessionEndpoint;
        internal IActiveSession? _activeSession;
        public String? Suffix { get; private set; }

        public void OnGet(String? Suffix)
        {
            this.Suffix= Suffix;
            _terminateSessionEndpoint=Url.ActionLink("TerminateSession", "Sample", new { Suffix });
            _activeSession=HttpContext.GetActiveSession();
        }
    }
}
