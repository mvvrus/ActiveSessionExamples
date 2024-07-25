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
        public SequenceParams? Params;
        public Boolean _runner_ok = false;
        public String StatusMessage { get; private set; } = "";
        public Task OnGetAsync([ModelBinder<ExtRunnerKeyMvcModelBinder>]ExtRunnerKey key)
        {
            IActiveSession active_session = HttpContext.GetActiveSession();
            if(!active_session.IsAvailable) {
                StatusMessage="Active session is unavailable.";
            }
            else {
                if(key.Generation!=active_session.Generation || key.ActiveSessionId!=active_session.Id) StatusMessage="Active session was replaced.";
                else {
                    var runner = active_session.GetSequenceRunner<SimSeqData>(key.RunnerNumber, HttpContext);
                    if(runner ==null) {
                        StatusMessage="Cannot contact a runner for the operation.";
                    }
                    else {
                        StatusMessage="Starting the runner operation.";
                        Params = runner.ExtraData as SequenceParams;
                        _runner_ok=true;
                        //TODO
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
