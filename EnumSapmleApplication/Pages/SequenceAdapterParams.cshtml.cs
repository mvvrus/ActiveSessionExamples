using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVVrus.AspNetCore.ActiveSession;
using MVVrus.AspNetCore.ActiveSession.StdRunner;
using SapmleApplication.Models;
using SapmleApplication.Sources;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SapmleApplication.Pages
{
    public class SequenceAdapterParamsModel : PageModel
    {
        [BindProperty]
        public BindParams? Input { get; set; }

        public readonly List<SelectListItem> Units = new List<SelectListItem>
        {
            new SelectListItem("sec","1000",true),
            new SelectListItem("msec","1")
        };

        public void OnGet(String mode)
        {
        }

        public ActionResult OnPost(String mode) 
        {
            if(ModelState.IsValid) {
                SequenceParams seq_params=MakeSequenceParams();
                IEnumerable<SimSeqData> source=new SyncDelayedEnumerble<SimSeqData>(seq_params.Stages, new SimSeqDataProducer().Sample);
                Int32 number;
                IRunner runner;
                (runner, number)= HttpContext.GetActiveSession().CreateSequenceRunner(source,HttpContext);
                runner.ExtraData=seq_params;
                return RedirectToPage("SequenceShowResults", new { runner_number = number });
            }
            return Page();
        }

        private SequenceParams MakeSequenceParams()
        {
            SequenceParams result = new SequenceParams(Input!.Stages!.Count);
            int ndx = 0;
            foreach(BindStage stage in Input!.Stages!) {
                result.Stages[ndx++]= new SimStage { 
                    Count=stage.Count!.Value, 
                    Delay=TimeSpan.FromMilliseconds(stage.Delay!.Value*stage.Scale) };

            }
            return result;
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
