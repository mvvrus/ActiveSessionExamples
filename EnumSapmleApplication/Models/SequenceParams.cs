namespace SapmleApplication.Models
{
    //View model for SequenceAdapterParams
    public class SequenceParams
    {
        public IReadOnlyList<SimStage> Stages { get; init; }
        public TimeSpan PollInterval { get; init; }
        public Int32? PollMaxCount { get; init; }
        public SyncMode Mode { get; init; }

        public SequenceParams(List<SimStage> Stages, TimeSpan PollInterval, Int32? PollMaxCount, SyncMode Mode)
        {
            this.Stages=Stages;
            this.PollInterval=PollInterval;
            this.PollMaxCount=PollMaxCount;
            this.Mode=Mode;
        }

        public enum SyncMode { sync, async};
    }
}
