using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVVrus.AspNetCore.ActiveSession;
using MVVrus.AspNetCore.ActiveSession.StdRunner;
using SampleApplication.Sources;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SampleApplication.Pages
{
    public class TimeSeriesParamsModel : PageModel
    {
        [DisplayName("Interval between measurements(sec)")]
        [Required]
        [Range(1,3600)]
        [BindProperty]
        public Int32 Interval { get; set; } = 10;

        public void OnGet()
        {
        }

        public ActionResult OnPost() 
        {
            if(ModelState.IsValid) {
                ExtRunnerKey key;
                IRunner runner;
                int runner_number;
                IActiveSession session = HttpContext.GetActiveSession();
                if(session.IsAvailable) {
                    RunnerRegistry registry = session.GetRegistry();
                    (runner, runner_number)= session.CreateTimeSeriesRunner(() => registry.Count, TimeSpan.FromSeconds(Interval), HttpContext);
                    runner.ExtraData=Interval;
                    key=(session, runner_number);
                    return RedirectToPage("TimeSeriesResults", new { key });
                }
                else return StatusCode(StatusCodes.Status500InternalServerError);
            }
            else return Page();
        }
    }
}
