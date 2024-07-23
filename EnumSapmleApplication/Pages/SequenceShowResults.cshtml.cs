using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVVrus.AspNetCore.ActiveSession;
using MVVrus.AspNetCore.ActiveSession.StdRunner;
using SampleApplication;
using SapmleApplication.Models;
using System.Text.RegularExpressions;
using SapmleApplication.Sources;

namespace SapmleApplication.Pages
{
    public class SequenceShowResultsModel : PageModel
    {
        public Task OnGetAsync([ModelBinder<RunnerKeyMvcModelBinder>]RunnerKey key)
        {
            IActiveSession active_session = HttpContext.GetActiveSession();
            if(!active_session.IsAvailable) {
                //TODO
                throw new NotImplementedException();
            }
            var runner = active_session.GetSequenceRunner<SimSeqData>(key, HttpContext);
            //TODO
            return Task.CompletedTask;
        }
    }
}
