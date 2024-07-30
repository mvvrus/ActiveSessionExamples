using MVVrus.AspNetCore.ActiveSession;

namespace SampleApplication.APIclasses
{
    public class GetAvailableRequest
    {
        public ExtRunnerKey RunnerKey { get; set; }
        public Int32? Advance { get; set; }
    }
}
