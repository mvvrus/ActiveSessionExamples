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
        public ActionResult<SampleSequenceResponse> GetAvailable(GetAvailableRequest Request)
        {
            IActiveSession session = HttpContext.GetActiveSession();
            if(session.IsAvailable && Request.RunnerKey.IsForSession(session)) {
                var runner = session.GetSequenceRunner<SimSeqData>(Request.RunnerKey.RunnerNumber, HttpContext);
                if(runner!=null) {
                    SampleSequenceResponse response = new SampleSequenceResponse();
                    response.backgroundProgress=runner.GetProgress().Progress;
                    response.isBackgroundExecutionCompleted=runner.IsBackgroundExecutionCompleted;
                    RunnerStatus runner_status;
                    (response.result, runner_status, response.position, response.exception) =
                        runner.GetAvailable(Request.Advance??Int32.MaxValue, TraceIdentifier:HttpContext.TraceIdentifier);
                    response.runnerStatus=runner_status.ToString();
                    return response;
                }
            }
            return StatusCode(StatusCodes.Status404NotFound);
        }

        [HttpPost("[action]")]
        public ActionResult<AbortResponse> Abort(AbortRequest Request)
        {
            IActiveSession session = HttpContext.GetActiveSession();
            if(session.IsAvailable && Request.RunnerKey.IsForSession(session)) {
                var runner = session.GetSequenceRunner<SimSeqData>(Request.RunnerKey.RunnerNumber, HttpContext);
                if(runner!=null) {
                    AbortResponse response = new AbortResponse();
                    response.runnerStatus=runner.Abort(HttpContext.TraceIdentifier).ToString();
                    return response;
                    
                }
            }
            return StatusCode(StatusCodes.Status404NotFound);
        }
    }
}
