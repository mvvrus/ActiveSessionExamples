namespace SampleApplication.APIclasses
{
    public class TimeSeriesRecordResponse : GetAvailableSequenceResponse<TimeSeriesRecordResponse.Measurement>
    {
        public record Measurement(String time, Int32 count);
    }
}
