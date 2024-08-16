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
                        runner.GetAvailable(Request.Advance??Int32.MaxValue, TraceIdentifier: HttpContext.TraceIdentifier);
                    response.runnerStatus=runner_status.ToString();
                    return response;
                }
            }
            return StatusCode(StatusCodes.Status410Gone);
        }

        [HttpPost("[action]")]
        public ActionResult<AbortResponse> Abort(AbortRequest Request)
        {
            IActiveSession session = HttpContext.GetActiveSession();
            if(session.IsAvailable && Request.RunnerKey.IsForSession(session)) {
                var runner = session.GetNonTypedRunner(Request.RunnerKey.RunnerNumber, HttpContext);
                if(runner!=null) {
                    AbortResponse response = new AbortResponse();
                    response.runnerStatus=runner.Abort(HttpContext.TraceIdentifier).ToString();
                    return response;

                }
            }
            return StatusCode(StatusCodes.Status410Gone);
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
                    IEnumerable<(DateTime Time, Int32 Count)> results;
                    (results, runner_status, response.position, response.exception) =
                        await runner.GetRequiredAsync(1, TraceIdentifier: HttpContext.TraceIdentifier);
                    response.runnerStatus=runner_status.ToString();
                    response.result=results.Select(x =>
                        new TimeSeriesRecordResponse.Measurement(time: x.Time.ToString("HH:mm:ss"), count: x.Count));
                    return response;
                }
            }
            return StatusCode(StatusCodes.Status410Gone);
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
            return StatusCode(StatusCodes.Status410Gone);
        }

        [HttpPost("[action]")]
        public ActionResult<ObserveResponse> GetAvailableObserve(ObserveRequest Request)
        {
            IActiveSession session = HttpContext.GetActiveSession();
            if(session.IsAvailable && Request.RunnerKey.IsForSession(session)) {
                IRunner<Int32>? runner = session.GetRunner<Int32>(Request.RunnerKey.RunnerNumber, HttpContext);
                if(runner!=null) {
                    ObserveResponse response = new ObserveResponse();
                    response.backgroundProgress=runner.GetProgress().Progress;
                    response.isBackgroundExecutionCompleted=runner.IsBackgroundExecutionCompleted;
                    RunnerStatus runner_status;
                    (response.result, runner_status, response.position, response.exception) =
                        runner.GetAvailable(TraceIdentifier: HttpContext.TraceIdentifier);
                    response.runnerStatus=runner_status.ToString();
                    return response;
                }
            }
            return StatusCode(StatusCodes.Status410Gone);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<ObserveResponse>> GetRequiredObserveAsync(ObserveRequiredRequest Request)
        {
            IActiveSession session = HttpContext.GetActiveSession();
            if(session.IsAvailable && Request.RunnerKey.IsForSession(session)) {
                IRunner<Int32>? runner = session.GetRunner<Int32>(Request.RunnerKey.RunnerNumber, HttpContext);
                if(runner!=null) {
                    ObserveResponse response = new ObserveResponse();
                    response.backgroundProgress=runner.GetProgress().Progress;
                    response.isBackgroundExecutionCompleted=runner.IsBackgroundExecutionCompleted;
                    RunnerStatus runner_status;
                    try {
                        CancellationTokenSource? effective_cts = null;
                        CancellationTokenSource timeout_cts = new CancellationTokenSource(Request.TimeoutSecs*1000);
                        effective_cts=CancellationTokenSource.CreateLinkedTokenSource(timeout_cts.Token, HttpContext.RequestAborted);
                        int advance=0, start_pos=Request.Target;
                        if(start_pos<0) { (advance, start_pos)=(start_pos, advance); }
                        try {
                            (response.result, runner_status, response.position, response.exception) =
                                await runner.GetRequiredAsync(advance, effective_cts.Token, start_pos, HttpContext.TraceIdentifier);
                        }
                        catch (OperationCanceledException) {
                            return StatusCode(StatusCodes.Status204NoContent);
//                            return StatusCode(StatusCodes.Status408RequestTimeout);
                        }
                        finally {
                            effective_cts?.Dispose();
                            timeout_cts.Dispose();
                        }
                        response.runnerStatus=runner_status.ToString();
                        return response;
                    }
                    catch(ArgumentException) {
                        return StatusCode(StatusCodes.Status422UnprocessableEntity);
                    }
                }
            }
            return StatusCode(StatusCodes.Status410Gone);
        }
    }
}
