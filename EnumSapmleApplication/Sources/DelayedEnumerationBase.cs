using SapmleApplication.Models;
namespace SapmleApplication.Sources
{
    public class DelayedEnumerationBase
    {
        readonly IReadOnlyList<SimStage> _stages;
        Int32 _currentStageIndex;
        Int32 _stageStep=-1; //Ajusted for the first MoveNext
        SimStage? CurrentStage { get =>
                _currentStageIndex>=0 && _currentStageIndex<_stages.Count? _stages[_currentStageIndex]:null; }

        public TimeSpan CurrentDelay { get => CurrentStage?.Delay??TimeSpan.Zero; }

        protected DelayedEnumerationBase(IReadOnlyList<SimStage> stages) { 
            _stages = stages??throw new ArgumentNullException(nameof(stages));
        }

        protected Boolean MakeStep()
        {
            if(CurrentStage!=null) {
                if(++_stageStep>=CurrentStage.Count) {
                    _stageStep = 0;
                    _currentStageIndex++;
                }
            }
            return CurrentStage!=null;
        }
    }
}
