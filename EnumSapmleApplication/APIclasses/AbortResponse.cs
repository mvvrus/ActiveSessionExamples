namespace SampleApplication.APIclasses
{
    public class AbortResponse
    {
        public Int32 status { get; init; } = StatusCodes.Status200OK;
        public String? runnerStatus { get; set; }
    }
}
