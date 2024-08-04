using Microsoft.Extensions.Primitives;
using MVVrus.AspNetCore.ActiveSession;
using System.Collections.Concurrent;

namespace SampleApplication.Sources
{
    public class RunnerRegistry: IDisposable
    {
        ConcurrentDictionary<IRunner,IRunner> _runnerList = new ();
        CancellationTokenSource? _cts = new CancellationTokenSource ();
        Object _lock=new Object ();

        public void RegisterRunner(IRunner Runner)
        {
            Runner.CompletionToken.Register(() => { _runnerList.TryRemove(Runner, out IRunner? _); NotifyChange();  });
            _runnerList.TryAdd(Runner,Runner);
            NotifyChange();
        }

        public void Dispose()
        {
            CancellationTokenSource? old_cts=Volatile.Read(in _cts); 
            old_cts?.Cancel();
            old_cts?.Dispose();
        }

        public Int32 Count=>_runnerList.Count;

        public IChangeToken GetOneTimeChangeToken()
        {
            CancellationTokenSource? cts = Volatile.Read(in _cts);
            return new CancellationChangeToken(cts?.Token??default);
        }

        void NotifyChange()
        {
            CancellationTokenSource new_cts = new CancellationTokenSource();
            CancellationTokenSource? old_cts=Interlocked.Exchange(ref _cts, new_cts); 
            old_cts?.Cancel();
            old_cts?.Dispose();
        }

    }
}
