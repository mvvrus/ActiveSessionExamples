namespace SampleApplication.APIclasses
{
    public class ObserveRequiredRequest: ObserveAvailableRequest
    {
        public Int32 Target { get; set; }
        public Int32 TimeoutSecs { get; set; }
        public Int32 Slot { get; set; }
    }
}
