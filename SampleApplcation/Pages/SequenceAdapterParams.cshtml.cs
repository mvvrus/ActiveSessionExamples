using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVVrus.AspNetCore.ActiveSession;
using MVVrus.AspNetCore.ActiveSession.StdRunner;
using SampleApplication.Sources;
using SapmleApplication.Models;
using SapmleApplication.Sources;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SapmleApplication.Pages
{
    public class SequenceAdapterParamsModel : PageModel
    {
        const string SYNC = "sync";
        const string ASYNC = "async";

        [BindProperty]
        public BindParams? Input { get; set; }
        [BindProperty]
        [BindRequired]
        public SequenceParams.SyncMode Mode { get; set; }

        public readonly List<SelectListItem> Units = new List<SelectListItem>
        {
            new SelectListItem("sec","1000",true),
            new SelectListItem("msec","1")
        };

        public SequenceAdapterParamsModel()
        {
            Input=new BindParams();
            Input.Stages=new List<BindStage> { new BindStage { Delay=0, Scale=1000 } };
        }

        public void OnGet(SequenceParams.SyncMode mode)
        {
            Mode=mode;
        }

        public ActionResult OnPost() 
        {
            if(ModelState.IsValid) {
                SequenceParams seq_params=MakeSequenceParams();
                IRunner runner;
                int runner_number;
                IActiveSession session= HttpContext.GetActiveSession();
                if(session.IsAvailable) {
                    switch(Mode) {
                        case SequenceParams.SyncMode.sync:
                            IEnumerable<SimSeqData> sync_source = new SyncDelayedEnumerble<SimSeqData>(seq_params.Stages, new SimSeqDataProducer().Sample);
                            (runner, runner_number)= session.CreateSequenceRunner(sync_source, HttpContext);
                            break;
                        case SequenceParams.SyncMode.async:
                            IAsyncEnumerable<SimSeqData> async_source = new AsyncDelayedEnumerble<SimSeqData>(seq_params.Stages, new SimSeqDataProducer().Sample);
                            (runner, runner_number) = session.CreateSequenceRunner(async_source, HttpContext);
                            break;
                        default:
                            return StatusCode(StatusCodes.Status400BadRequest);
                    }
                    session.GetRegistry().RegisterRunner(runner);
                    ExtRunnerKey key = (session, runner_number);
                    runner.ExtraData=seq_params;
                    return RedirectToPage("SequenceShowResults", new { key });
                }
                else return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Page();
        }

        private SequenceParams MakeSequenceParams()
        {
            List<SimStage> stages = new List<SimStage>(Input!.Stages!.Count);   
            foreach(BindStage stage in Input!.Stages!) {
                stages.Add(new SimStage
                {
                    Count=stage.Count!.Value,
                    Delay=TimeSpan.FromMilliseconds(stage.Delay!.Value*stage.Scale)
                });
            }
            return new SequenceParams(stages, TimeSpan.FromSeconds(Input.PollInterval!.Value), Input.PollMaxCount, Mode, Input.StartCount!.Value);
        }

        public List<String>? CustomDisplayValidationErrors()
        {
            if(ModelState.IsValid) {
                return null;
            }
            else {
                List<String> result = new List<string>();
                Regex index_pattern = new Regex(@".*\[(\d+)\].*");
                foreach(var kvp in ModelState) {
                    String prefix = "";
                    Match match= index_pattern.Match(kvp.Key);
                    if(match.Success) prefix=match.Groups[1].Value+": ";
                    if(kvp.Value.ValidationState==ModelValidationState.Invalid) {
                        foreach(var error in kvp.Value.Errors) {
                            result.Add(prefix+error.ErrorMessage);
                        }
                        
                    }
                }
                return result;
            }

        }

        public class BindParams
        {
            public List<BindStage>? Stages { get; set; }
            [Required]
            [Range(1, 60)]
            [DisplayName("Poll interval (sec):")]
            public Int32? PollInterval { get; set; } = 1;
            [DisplayName("Max records per poll:")]
            public Int32? PollMaxCount { get; set; }
            [Required]
            [Range(1, 40)]
            [DisplayName("Records to fetch at start:")]
            public Int32? StartCount { get; set; } = 10;
        }

        public class BindStage
        {
            [Required]
            public Int32? Count { get; set; }
            [Required]
            public Int32? Delay { get; set; }
            [DisplayName("Units")]
            public Int32 Scale { get; set; }
        }

    }
}
