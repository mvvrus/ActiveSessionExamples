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

        [HttpPost("[action]")]
        public IActionResult TerminateSession()
        {
            IActiveSession session = HttpContext.GetActiveSession();
            session.Terminate(HttpContext);
            return StatusCode(StatusCodes.Status204NoContent);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<TimeSeriesRecordResponse>> GetTimeSeriesRecordAsync(TimeSeriesRecordRequest Request)
        {
            IActiveSession session = HttpContext.GetActiveSession();
            if(session.IsAvailable && Request.RunnerKey.IsForSession(session)) {
                var runner = session.GetTimeSeriesRunner<Int32>(Request.RunnerKey.RunnerNumber, HttpContext);
                if(runner!=null) {
                    TimeSeriesRecordResponse response = new TimeSeriesRecordResponse();
                    response.backgroundProgress=runner.GetProgress().Progress;
                    response.isBackgroundExecutionCompleted=runner.IsBackgroundExecutionCompleted;
                    RunnerStatus runner_status;
                    IEnumerable<(DateTime Time,Int32 Count)> results;
                    (results, runner_status, response.position, response.exception) =
                        await runner.GetRequiredAsync(1, TraceIdentifier: HttpContext.TraceIdentifier);
                    response.runnerStatus=runner_status.ToString();
                    response.result=results.Select(x=> 
                        new TimeSeriesRecordResponse.Measurement(time:x.Time.ToString("HH:mm:ss"),count:x.Count ));
                    return response;
                }
            }
            return StatusCode(StatusCodes.Status404NotFound);
        }

        [HttpPost("[action]")]
        public ActionResult<TimeSeriesRecordResponse> GetTimeSeriesAvailRecords(TimeSeriesRecordRequest Request)
        {
            IActiveSession session = HttpContext.GetActiveSession();
            if(session.IsAvailable && Request.RunnerKey.IsForSession(session)) {
                var runner = session.GetTimeSeriesRunner<Int32>(Request.RunnerKey.RunnerNumber, HttpContext);
                if(runner!=null) {
                    TimeSeriesRecordResponse response = new TimeSeriesRecordResponse();
                    response.backgroundProgress=runner.GetProgress().Progress;
                    response.isBackgroundExecutionCompleted=runner.IsBackgroundExecutionCompleted;
                    RunnerStatus runner_status;
                    IEnumerable<(DateTime Time, Int32 Count)> results;
                    (results, runner_status, response.position, response.exception) =
                        runner.GetAvailable(TraceIdentifier: HttpContext.TraceIdentifier);
                    response.runnerStatus=runner_status.ToString();
                    response.result=results.Select(x =>
                        new TimeSeriesRecordResponse.Measurement(time: x.Time.ToString("HH:mm:ss"), count: x.Count));
                    return response;
                }
            }
            return StatusCode(StatusCodes.Status404NotFound);
        }
    }
}
