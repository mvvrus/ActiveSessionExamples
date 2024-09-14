using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVVrus.AspNetCore.ActiveSession;
using MVVrus.AspNetCore.ActiveSession.StdRunner;
using SampleApplication.Sources;
using SapmleApplication.Models;
using SapmleApplication.Sources;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SampleApplication.Pages
{
    public class ExclusiveParamsModel : PageModel
    {
        internal Int32 _startCount = 10;
        internal Int32 _pollInterval = 1;
        internal String _key;
        [BindProperty]
        [Required]
        [Range(1,Int32.MaxValue)]
        [DisplayName("Count")]
        public Int32? AsyncCount { get; set; }  = 20;

        readonly ISessionServiceLock<IExclusiveService> _sessionServiceLock;

        internal List<SimStage> _stages =new List<SimStage>(2);

        public ExclusiveParamsModel(ISessionServiceLock<IExclusiveService> SessionServiceLock)
        {
            _stages.Add(new SimStage() { Count=_startCount, Delay=TimeSpan.Zero });
            _stages.Add(new SimStage() { Count=AsyncCount.Value, Delay=TimeSpan.FromSeconds(1) });
            _key = Guid.NewGuid().ToString();
            _sessionServiceLock=SessionServiceLock;
        }

        public void OnGet()
        {
        }

        public async Task<ActionResult> OnPostAsync() 
        {
            _stages[_stages.Count-1].Count=AsyncCount??0;
            if(ModelState.IsValid) {
                IRunner runner;
                int runner_number;
                IActiveSession session = HttpContext.GetActiveSession();
                if(session.IsAvailable) {
                    ILockedSessionService<IExclusiveService>? accessor;
                    try {
                        accessor =  await _sessionServiceLock.AcquireAsync(Timeout.InfiniteTimeSpan, HttpContext.RequestAborted);
                    }
                    catch(ObjectDisposedException) {
                        return RedirectToPage("SessionGone");
                    }
                    if(accessor == null) return StatusCode(StatusCodes.Status500InternalServerError);

                    SequenceParams seq_params =
                        new SequenceParams(_stages, TimeSpan.FromSeconds(_pollInterval), null, SequenceParams.SyncMode.async, _startCount);
                    IAsyncEnumerable<SimSeqData> async_source = new AsyncDelayedEnumerble<SimSeqData>(seq_params.Stages, new SimSeqDataProducer().Sample);
                    (runner, runner_number) = session.CreateSequenceRunner(async_source, HttpContext, accessor);
                    session.GetRegistry().RegisterRunner(runner);
                    ExtRunnerKey key = (session, runner_number);
                    runner.ExtraData=seq_params;
                    return RedirectToPage("SequenceShowResults", new { key });
                }
                else return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Page();
        }
    }
}
