using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVVrus.AspNetCore.ActiveSession;
using MVVrus.AspNetCore.ActiveSession.StdRunner;

namespace SampleApplication.Pages
{
    public class TimeSeriesResultsModel : PageModel
    {
        internal ExtRunnerKey _key;
        internal Int32 _intervalSecs;
        internal List<(DateTime Time, Int32 Count)> _results = new List<(DateTime Time, Int32 Count)>();
        internal RunnerStatus _status;
        internal Int32 _position;
        internal Exception? _exception;
        internal String RUNNER_COMPLETED = "Collection of data from runner has been stopped.";
        internal Int32 _bkgProgress;
        internal Boolean _bkgIsCompleted;
        internal Int32 _timeoutMsecs;
        internal String? _AbortEndpoint;
        internal String? _GetTimeSeriesRecordEndpoint; //TODO Add API controller and set up the endpoint GetTimeSeriesRecordAsync

        public String StartupStatusMessage { get; private set; } = "";

        public async Task OnGetAsync([ModelBinder<ExtRunnerKeyMvcModelBinder>] ExtRunnerKey Key)
        {
            _AbortEndpoint=Url.ActionLink("Abort", "Sample");
            _GetTimeSeriesRecordEndpoint=Url.ActionLink("GetTimeSeriesRecord", "Sample");
            _key=Key;
            IActiveSession active_session = HttpContext.GetActiveSession();
            if(!active_session.IsAvailable) {
                StartupStatusMessage="Active session is unavailable.";
            }
            else {
                if(Key.Generation!=active_session.Generation || Key.ActiveSessionId!=active_session.Id) StartupStatusMessage="Active session was replaced.";
                else {
                    var runner = active_session.GetTimeSeriesRunner<Int32>(Key.RunnerNumber, HttpContext);
                    if(runner ==null) {
                        StartupStatusMessage="Cannot find a runner.";
                    }
                    else {
                        _intervalSecs = (Int32)runner.ExtraData!;
                        _timeoutMsecs = _intervalSecs*1000 - 100;
                        IEnumerable<(DateTime Time, Int32 Count)> timeint_enum;
                        (timeint_enum, _status, _position, _exception) =
                            await runner.GetRequiredAsync(1, TraceIdentifier: HttpContext.TraceIdentifier);
                        _results = timeint_enum.ToList();
                        if(_status.IsFinal()) StartupStatusMessage=RUNNER_COMPLETED;
                        else {
                            StartupStatusMessage="Collecting data from the runner.";
                        }
                        _bkgIsCompleted = runner.IsBackgroundExecutionCompleted;
                        _bkgProgress = runner.GetProgress().Progress;
                    }
                }
            }
        }
    }
}
