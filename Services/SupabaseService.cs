using System.Net.Http;

public class SupabaseService
{
    private readonly HttpClient _http;

    private const string url = "https://njtfakaledathlsreizf.supabase.co";
    private const string key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im5qdGZha2FsZWRhdGhsc3JlaXpmIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzQ2MzQyNTUsImV4cCI6MjA5MDIxMDI1NX0.vi2EvSNzZ-5tS7-eUeJJTsQVdXEnnfYo3iRee6a77pY";

    public SupabaseService(HttpClient http)
    {
        _http = http;
        _http.DefaultRequestHeaders.Add("apikey", key);
        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {key}");
    }

   public async Task<bool> Login(string email, string password)
{
    var encodedEmail = Uri.EscapeDataString(email);
    var encodedPassword = Uri.EscapeDataString(password);

    var response = await _http.GetAsync(
        $"{url}/rest/v1/users?email=eq.{encodedEmail}&password=eq.{encodedPassword}"
    );

    var data = await response.Content.ReadAsStringAsync();

    return data != "[]";
}
}