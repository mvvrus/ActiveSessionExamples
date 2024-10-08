﻿using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;

namespace SampleApplication.Sources
{
    public class RunnerRegistryObserver:IDisposable
    {
        RunnerRegistry _registry;
        IDisposable _observationTokenRegistration;
        TaskCompletionSource<Int32> _currentWaitSource = new TaskCompletionSource<Int32>(); 
        List<CallbackInfo> _callbackInfos=new();



        public RunnerRegistryObserver(RunnerRegistry Registry) 
        { 
            _registry = Registry;
            _observationTokenRegistration = ChangeToken.OnChange(_registry.GetOneTimeChangeToken, RegistryChanged);
        }

        public async Task Observe(Action<Int32, Int32?> Callback, CancellationToken CompletionToken)
        {
            TaskCompletionSource<Int32> wait_source;
            TaskCompletionSource<Int32> completion_source =new TaskCompletionSource<Int32>();
            Callback.Invoke(_registry.Count, null);
            using(
                CancellationTokenRegistration completion_registration= CompletionToken.Register(
                    ()=>completion_source.SetCanceled(CompletionToken))
            ) {
                while(true) {
                    wait_source = Volatile.Read(in _currentWaitSource);
                    Int32 count = (await Task.WhenAny(wait_source.Task,completion_source.Task)).Result;
                    //One can come here only if wait_source.Task is ran to completion,
                    // because completion_source.Task never runs to completion, it can be completed via an OperationCanceledException only.
                    Callback(count, null);
                }
            }
            //One never come here to run this task to completion, as a loop above can be be terminated by a OperationCanceledException
            //This exception will be intercepted by calling code as an expected one.
        }

        void RegistryChanged()
        {
            TaskCompletionSource<Int32> new_wait_source = new TaskCompletionSource<Int32>();
            TaskCompletionSource<Int32> old_wait_source = Interlocked.Exchange(ref _currentWaitSource, new_wait_source);
            old_wait_source.SetResult(_registry.Count);
        }

        public void Dispose()
        {   
            _observationTokenRegistration.Dispose();
        }

        class  CallbackInfo
        {
            public Action<Int32, Int32?> Callback { get; init; }
            public CancellationToken CompletionToken { get; init; }

            public CallbackInfo(Action<Int32, Int32?> Callback, CancellationToken CompletionToken)
            {
                this.Callback=Callback;
                this.CompletionToken=CompletionToken;
            }
        }
    }
}
