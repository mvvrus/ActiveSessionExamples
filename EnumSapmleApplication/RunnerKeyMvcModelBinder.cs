using Microsoft.AspNetCore.Mvc.ModelBinding;
using MVVrus.AspNetCore.ActiveSession;
using static SampleApplication.RunnerKeyExtensions;

namespace SampleApplication
{
    public class RunnerKeyMvcModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext Context)
        {
            String name = Context.ModelName;
            String? key_string = Context.ValueProvider.GetValue(name).FirstOrDefault();
            if(key_string != null) {
                RunnerKey key;
                if(key_string.TryParse(out key)) {
                    Context.ModelState.SetModelValue(name, key, key_string);
                    Context.Result=ModelBindingResult.Success(key);
                }
            }
            return Task.CompletedTask;
        }
    }
}
