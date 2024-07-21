namespace SapmleApplication.Models
{
    //View model for SequenceAdapterParams
    public class SequenceParams
    {
        public SimStage[] Stages { get; init; }
        public SequenceParams(Int32 num_stages)
        {
            Stages=new SimStage[num_stages];
        }

    }
}
