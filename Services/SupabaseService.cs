using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using CourseSelectionGuide.Models;
using System.Text.Json;

public class SupabaseService
{
    private readonly HttpClient _http;

    private const string url = "https://njtfakaledathlsreizf.supabase.co";
    private const string key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im5qdGZha2FsZWRhdGhsc3JlaXpmIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzQ2MzQyNTUsImV4cCI6MjA5MDIxMDI1NX0.vi2EvSNzZ-5tS7-eUeJJTsQVdXEnnfYo3iRee6a77pY";

    public SupabaseService(HttpClient http)
    {
        _http = http;
    }

   public async Task<Users?> Login(string email, string password)
    {
        var encodedEmail = Uri.EscapeDataString(email);
        var response = await _http.GetAsync(
            $"{url}/rest/v1/Users?Email=eq.{encodedEmail}&select=*"
        );

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<List<Users>>(json);
        var user = users?.FirstOrDefault();

        if (user != null && user.Password == password)
            return user;

        return null;
    }
}