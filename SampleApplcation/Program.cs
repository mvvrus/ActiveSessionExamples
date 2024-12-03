using Microsoft.AspNetCore.Mvc.Rendering;
using MVVrus.AspNetCore.ActiveSession;
using MVVrus.AspNetCore.ActiveSession.StdRunner;
using SampleApplication.Sources;
using SapmleApplication.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddEnumAdapter<SimSeqData>();
builder.Services.AddAsyncEnumAdapter<SimSeqData>();
builder.Services.AddTimeSeriesRunner<Int32>();
builder.Services.AddSessionProcessRunner<Int32>();

builder.Services.AddRazorPages().AddViewOptions(options => options.HtmlHelperOptions.FormInputRenderMode=FormInputRenderMode.AlwaysUseCurrentCulture);
builder.Services.AddControllers();
builder.Services.AddScoped<IExclusiveService, ExclusiveService>();

WebApplication app = builder.Build();

app.UseSession();
app.UseActiveSessions();
app.UseActiveSessions(CheckRoutingSuffix);
app.MapRazorPages();
app.MapControllers();

app.Run();

const String SUFFIX = "suffix";
(Boolean InScope,String? Suffix) CheckRoutingSuffix(HttpContext Context)
{
    RouteValueDictionary? route_values = Context.GetRouteData().Values;
    if(route_values is not null && route_values.ContainsKey(SUFFIX)) {
        return (true, (route_values[SUFFIX] as String)?.ToLower());
    }
    else return (false, null);
}
