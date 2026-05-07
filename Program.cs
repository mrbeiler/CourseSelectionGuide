using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CourseSelectionGuide;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im5qdGZha2FsZWRhdGhsc3JlaXpmIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzQ2MzQyNTUsImV4cCI6MjA5MDIxMDI1NX0.vi2EvSNzZ-5tS7-eUeJJTsQVdXEnnfYo3iRee6a77pY";

builder.Services.AddHttpClient<SupabaseService>(client =>
{
    client.DefaultRequestHeaders.Add("apikey", supabaseKey);
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseKey}");
});

builder.Services.AddHttpClient<ClassListService>(client =>
{
    client.DefaultRequestHeaders.Add("apikey", supabaseKey);
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseKey}");
});

builder.Services.AddSingleton<UserSession>();
builder.Services.AddScoped<SessionStorageService>();

await builder.Build().RunAsync();