using SapmleApplication.Models;
using System.Collections;

namespace SapmleApplication.Sources
{
    public class SyncDelayedEnumerble<T> : DelayedEnumerationBase, IEnumerable<T>, IEnumerator<T>
    {
        Int32 _currentStep = -1;
        readonly Func<Int32, T> _stepResultFunc;

        public SyncDelayedEnumerble(SimStage[] stages, Func<Int32,T> step_result_func) : base(stages)
        {
            _stepResultFunc= step_result_func;
        }

        public T Current { get => _stepResultFunc(Math.Min(_currentStep,0)); }

        public Boolean MoveNext()
        {
            TimeSpan delay=CurrentDelay;
            if(delay>TimeSpan.Zero) Thread.Sleep(delay);
            Boolean result = MakeStep();
            if(result) _currentStep++;
            return result;
        }

        public IEnumerator<T> GetEnumerator() { return this; }

        public void Dispose() { }
        public void Reset() { throw new NotImplementedException(); }
        Object IEnumerator.Current => Current!;
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}
