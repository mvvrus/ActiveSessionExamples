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
        internal SequenceParams? _params;
        internal List<SimSeqData> _results=new List<SimSeqData>();
        internal RunnerStatus _status;
        internal Int32 _position;
        internal Exception? _exception;
        internal ExtRunnerKey _key;
        internal String? _GetAvailableEndpoint;
        internal String? _AbortEndpoint;
        internal String RUNNER_COMPLETED = "The runner is completed.";
        internal String RUNNER_RUNNING = "The runner is running in background.";
        internal Int32 _bkgProgress;
        internal Boolean _bkgIsCompleted;

        public String StartupStatusMessage { get; private set; } = "";
        

        public async Task OnGetAsync([ModelBinder<ExtRunnerKeyMvcModelBinder>]ExtRunnerKey Key)
        {
            _GetAvailableEndpoint=Url.ActionLink("GetAvailable","Sample");
            _AbortEndpoint=Url.ActionLink("Abort", "Sample");
            _key=Key;
            IActiveSession active_session = HttpContext.GetActiveSession();
            if(!active_session.IsAvailable) {
                StartupStatusMessage="Active session is unavailable.";
            }
            else {
                if(Key.Generation!=active_session.Generation || Key.ActiveSessionId!=active_session.Id) StartupStatusMessage="Active session was replaced.";
                else {
                    var runner = active_session.GetSequenceRunner<SimSeqData>(Key.RunnerNumber, HttpContext);
                    if(runner ==null) {
                        StartupStatusMessage="Cannot find a runner.";
                    }
                    else {
                        _params = runner.ExtraData as SequenceParams;
                        IEnumerable<SimSeqData> res_enum;
                        (res_enum,_status,_position,_exception) = 
                            await runner.GetRequiredAsync(_params?.StartCount??IRunner.DEFAULT_ADVANCE, TraceIdentifier: HttpContext.TraceIdentifier);
                        _results = res_enum.ToList();
                        if(_status.IsFinal()) StartupStatusMessage=RUNNER_COMPLETED;
                        else {
                            StartupStatusMessage="The runner is running in background.";
                        }
                        _bkgIsCompleted = runner.IsBackgroundExecutionCompleted;
                        _bkgProgress = runner.GetProgress().Progress;
                    }
                }
            }
        }

        public String SmartInterval(TimeSpan Interval)
        {
            if(Interval==TimeSpan.Zero) return "0";
            else if(Interval<TimeSpan.FromSeconds(1)) return Interval.TotalMilliseconds.ToString("F0")+"ms";
            else return Interval.TotalSeconds.ToString("F1")+"s";
        }
    }
}
