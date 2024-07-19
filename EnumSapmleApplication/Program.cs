using Microsoft.AspNetCore.Mvc.Rendering;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages().AddViewOptions(options=>options.HtmlHelperOptions.FormInputRenderMode=FormInputRenderMode.AlwaysUseCurrentCulture);
var app = builder.Build();

//app.MapGet("/", () => "Hello World!");
app.MapRazorPages();

app.Run();
