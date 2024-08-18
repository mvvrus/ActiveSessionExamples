using MVVrus.AspNetCore.ActiveSession;

namespace SampleApplication.Sources
{
    public static class RunnerRegistryActiveSessionsExtensions
    {
        public const string REGISTRY_NAME = "RunnerRegistry";
        public const string OBSERVER_NAME = "RunnerRegistryObserver";

        public static RunnerRegistry GetRegistry(this IActiveSession ActiveSession)
        {
            Object? cached_result;
            RunnerRegistry? result = null;
            //First just try to get the existing registry (fast path)
            if(!ActiveSession.Properties.TryGetValue(REGISTRY_NAME, out cached_result)) { //Fast path isn't available
                lock(ActiveSession.Properties) { //Lock shareble resource - Properties dictionary
                    result = new RunnerRegistry(); //Create new registry to add it to Properties
                    if(ActiveSession.Properties.TryAdd(REGISTRY_NAME, result)) //Use "double check pattern" with second check within lock block
                        //Addition was successful
                        ActiveSession.CleanupCompletionTask.ContinueWith((_) => result.Dispose()); //Plan disposing the registry added after ebd of ActiveSession
                    else {
                        //Somebody added rigistry already between checks
                        cached_result = ActiveSession.Properties[REGISTRY_NAME]; //Get previosly added registry
                        result.Dispose();
                        result=null; //Dispose and clear result
                    }
                }
            }
            return result??(RunnerRegistry)cached_result!; //cached_result is not null here;
        }

        public static RunnerRegistryObserver GetRegistryObserver(this IActiveSession ActiveSession)
        {
            RunnerRegistry registry = ActiveSession.GetRegistry();
            Object? cached_result;
            RunnerRegistryObserver? result = null;
            //First just try to get the existing registry observer (fast path)
            if(!ActiveSession.Properties.TryGetValue(OBSERVER_NAME, out cached_result)) { //Fast path isn't available
                lock(ActiveSession.Properties) { //Lock shareble resource - Properties dictionary
                    result = new RunnerRegistryObserver(registry); //Create new registry observer to add it to Properties
                    if(ActiveSession.Properties.TryAdd(OBSERVER_NAME, result)) //Use "double check pattern" with second check within lock block
                        //Addition was successful
                        ActiveSession.CleanupCompletionTask.ContinueWith((_) => result.Dispose()); //Plan disposing the observer added after ebd of ActiveSession
                    else {
                        //Somebody added rigistry already between checks
                        cached_result = ActiveSession.Properties[OBSERVER_NAME]; //Get previosly added registry
                        result.Dispose();
                        result=null; //Dispose and clear result
                    }
                }
            }
            return result??(RunnerRegistryObserver)cached_result!; //cached_result is not null here;
        }


    }
}
