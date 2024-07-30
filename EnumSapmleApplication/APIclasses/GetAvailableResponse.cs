using MVVrus.AspNetCore.ActiveSession;

namespace SampleApplication.APIclasses
{
    public class GetAvailableResponse<TResult>
    {
        public Int32 status { get; init; } = StatusCodes.Status200OK;
        public String? runnerStatus { get; set; }
        public Int32 position { get; set; }
        public Exception? exception { get; set; }
        public TResult? result { get; set; }
        public Boolean isBackgroundExecutionCompleted { get; set; }
        public Int32 backgroundProgress { get; set; }
    }
}
