using Microsoft.AspNetCore.Mvc.ModelBinding;
using MVVrus.AspNetCore.ActiveSession;

namespace SampleApplication
{
    public class ExtRunnerKeyMvcModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext Context)
        {
            String name = Context.ModelName;
            String? key_string = Context.ValueProvider.GetValue(name).FirstOrDefault();
            if(key_string != null) {
                ExtRunnerKey key;
                if(ExtRunnerKey.TryParse(key_string, out key)) {
                    Context.ModelState.SetModelValue(name, key, key_string);
                    Context.Result=ModelBindingResult.Success(key);
                }
            }
            return Task.CompletedTask;
        }
    }
}
