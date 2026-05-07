using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CourseSelectionGuide;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddHttpClient<SupabaseService>(client =>
{
    var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im5qdGZha2FsZWRhdGhsc3JlaXpmIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzQ2MzQyNTUsImV4cCI6MjA5MDIxMDI1NX0.vi2EvSNzZ-5tS7-eUeJJTsQVdXEnnfYo3iRee6a77pY";
    client.DefaultRequestHeaders.Add("apikey", key);
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {key}");
});

builder.Services.AddScoped<ClassListService>();

builder.Services.AddSingleton<UserSession>();

await builder.Build().RunAsync();
