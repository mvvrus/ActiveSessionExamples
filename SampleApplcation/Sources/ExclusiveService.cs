using MVVrus.AspNetCore.ActiveSession;

namespace SampleApplication.Sources
{
    public class ExclusiveService : IExclusiveService
    {
        readonly String? _activeSessionId;
        readonly Int32 _generation;

        public ExclusiveService(IHttpContextAccessor? ContextAccessor)
        {
            HttpContext? context = ContextAccessor?.HttpContext;
            IActiveSession? session = context?.GetActiveSession();
            if(!session?.IsAvailable ?? true) session=null;
            _activeSessionId = session?.Id;
            _generation = session?.Generation ?? 0;
        }

        public String GetSessionId()
        {
            return _activeSessionId!=null ? _activeSessionId+":"+_generation.ToString() : "unknown";
        }
    }
}
