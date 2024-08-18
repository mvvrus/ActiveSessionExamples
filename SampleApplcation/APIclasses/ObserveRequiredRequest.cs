namespace SampleApplication.APIclasses
{
    public class ObserveRequiredRequest: ObserveRequest
    {
        public Int32 Target { get; set; }
        public Int32 TimeoutSecs { get; set; }
    }
}
