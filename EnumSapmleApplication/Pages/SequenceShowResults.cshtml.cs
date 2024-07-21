using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVVrus.AspNetCore.ActiveSession;
using MVVrus.AspNetCore.ActiveSession.StdRunner;
using SapmleApplication.Models;
using SapmleApplication.Sources;

namespace SapmleApplication.Pages
{
    public class SequenceShowResultsModel : PageModel
    {
        public void OnGet(int runner_number)
        {
            IActiveSession active_session = HttpContext.GetActiveSession();
            if(!active_session.IsAvailable) {
                //TODO
                throw new NotImplementedException();
            }
            var runner = active_session.GetSequenceRunner<SimSeqData>(runner_number, HttpContext);
            //TODO


        }
    }
}
