using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVVrus.AspNetCore.ActiveSession;
using MVVrus.AspNetCore.ActiveSession.StdRunner;
using SampleApplication.Sources;

namespace SampleApplication.Pages
{
    public class ObservationResultsModel : PageModel
    {
        internal Int32 _result;
        internal RunnerStatus _status;
        internal Int32 _position;
        internal Exception? _exception;
        internal ExtRunnerKey _key;
        internal Boolean _sessionAvailable;
        internal String? _AbortEndpoint;
        public String StartupStatusMessage { get; private set; } = "";

        public void OnGet()
        {
            _AbortEndpoint=Url.ActionLink("Abort", "Sample");
            IActiveSession active_session = HttpContext.GetActiveSession();
            if(!active_session.IsAvailable) {
                StartupStatusMessage="Active session is unavailable.";
            }
            else {
                _sessionAvailable=true;
                StartupStatusMessage="Observation runner is active.";
                RunnerRegistryObserver observer = active_session.GetRegistryObserver();
                (IRunner<Int32> runner, Int32 runner_number) = active_session.CreateSessionProcessRunner<Int32>(observer.Observe, HttpContext);
                _key=(active_session,runner_number);
                (_result, _status, _position, _exception)=runner.GetAvailable(TraceIdentifier: HttpContext.TraceIdentifier);
            }
        }
    }
}
