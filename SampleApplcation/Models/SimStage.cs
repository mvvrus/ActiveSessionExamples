namespace SapmleApplication.Models
{
    public record SimStage
    {
        public Int32 Count { get; set; }
        public TimeSpan Delay { get; set; }
    }
}
