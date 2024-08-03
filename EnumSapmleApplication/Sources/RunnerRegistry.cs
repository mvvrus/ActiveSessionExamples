using MVVrus.AspNetCore.ActiveSession;
using System.Collections.Concurrent;

namespace SampleApplication.Sources
{
    public class RunnerRegistry: IDisposable
    {
        ConcurrentDictionary<IRunner,IRunner> _runnerList = new ();

        public void RegisterRunner(IRunner Runner)
        {
            Runner.CompletionToken.Register(() => _runnerList.TryRemove(Runner, out IRunner? _));
            _runnerList.TryAdd(Runner,Runner);
        }

        public void Dispose()
        {
            //TODO Planned to release change token to be defined
        }

        public Int32 Count=>_runnerList.Count;

    }
}
