using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CourseSelectionGuide;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<SupabaseService>();

builder.Services.AddHttpClient<SupabaseService>(client =>
{
    client.DefaultRequestHeaders.Add("apikey", builder.Configuration["Supabase:Key"]);
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {builder.Configuration["Supabase:Key"]}");
});

builder.Services.AddScoped<ClassListService>();

await builder.Build().RunAsync();
