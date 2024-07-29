using MVVrus.AspNetCore.ActiveSession;

namespace SampleApplication.APIclasses
{
    public class GetAvailResponce<TResult>
    {
        public Int32 Status { get; init; } = StatusCodes.Status200OK;
        public String? RunnerStatus { get; set; }
        public Int32 Position { get; set; }
        public Exception? Exception { get; set; }
        public TResult? Result { get; set; }
    }
}
