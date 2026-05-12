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

    public async Task<Users?> Signup(string name, string email, string password, string grade)
    {
        // First check if email already exists
        var encodedEmail = Uri.EscapeDataString(email);
        var checkResponse = await _http.GetAsync(
            $"{url}/rest/v1/Users?Email=eq.{encodedEmail}&select=id"
        );

        if (checkResponse.IsSuccessStatusCode)
        {
            var json = await checkResponse.Content.ReadAsStringAsync();
            var existing = JsonSerializer.Deserialize<List<Users>>(json);
            if (existing?.Any() == true)
                return null; // Email already exists
        }

        // Create new user without id field - use PascalCase to match database columns
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null // Use exact property names (PascalCase)
        };

        var newUserData = new
        {
            Name = name,
            Email = email,
            Password = password,
            Grade = grade
        };

        var jsonString = JsonSerializer.Serialize(newUserData, jsonOptions);
        var body = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, $"{url}/rest/v1/Users")
        {
            Content = body
        };
        request.Headers.Add("Prefer", "return=representation");

        var response = await _http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return null;

        var responseJson = await response.Content.ReadAsStringAsync();
        var createdUsers = JsonSerializer.Deserialize<List<Users>>(responseJson);
        return createdUsers?.FirstOrDefault();
    }
}