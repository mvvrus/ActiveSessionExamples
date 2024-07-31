using SapmleApplication.Models;
using System.Collections;

namespace SapmleApplication.Sources
{
    public class AsyncDelayedEnumerble<T> : DelayedEnumerationBase, IAsyncEnumerable<T>, IAsyncEnumerator<T>
    {
        Int32 _currentStep = -1;
        readonly Func<Int32, T> _stepResultFunc;
        CancellationToken _cancellationToken;

        public AsyncDelayedEnumerble(IReadOnlyList<SimStage> stages, Func<Int32, T> step_result_func) : base(stages)
        {
            _stepResultFunc= step_result_func;
        }

        public T Current { get => _stepResultFunc(Math.Max(_currentStep, 0)); }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken CancellationToken = default)
        {
            _cancellationToken=CancellationToken;
            return this; 
        }

        public async ValueTask<Boolean> MoveNextAsync()
        {
            _cancellationToken.ThrowIfCancellationRequested();
            TimeSpan delay = CurrentDelay;
            if(delay>TimeSpan.Zero) await Task.Delay(delay,_cancellationToken);
            Boolean result = MakeStep();
            if(result) _currentStep++;
            return result;
        }
    }
}
