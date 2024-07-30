using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVVrus.AspNetCore.ActiveSession;
using MVVrus.AspNetCore.ActiveSession.StdRunner;
using SampleApplication.APIclasses;
using SapmleApplication.Models;

namespace SampleApplication.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        [HttpPost("[action]")]
        public ActionResult<SampleSequenceResponce> GetAvailable(GetAvailableRequest Request)
        {
            IActiveSession session = HttpContext.GetActiveSession();
            if(session.IsAvailable && Request.RunnerKey.IsForSession(session)) {
                var runner = session.GetSequenceRunner<SimSeqData>(Request.RunnerKey.RunnerNumber, HttpContext);
                if(runner!=null) {
                    SampleSequenceResponce responce = new SampleSequenceResponce();
                    responce.backgroundProgress=runner.GetProgress().Progress;
                    responce.isBackgroundExecutionCompleted=runner.IsBackgroundExecutionCompleted;
                    RunnerStatus runner_status;
                    (responce.result, runner_status, responce.position, responce.exception) =
                        runner.GetAvailable(Request.Advance??Int32.MaxValue, TraceIdentifier:HttpContext.TraceIdentifier);
                    responce.runnerStatus=runner_status.ToString();
                    return responce;
                }
            }
            return StatusCode(StatusCodes.Status404NotFound);
        }
    }
}
