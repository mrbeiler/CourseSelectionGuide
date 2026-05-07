using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using CourseSelectionGuide.Models;
using System.Text.Json;

public class ClassListService
{
    private readonly HttpClient _http;
    private const string BaseUrl = "https://njtfakaledathlsreizf.supabase.co/rest/v1";

    public ClassListService(HttpClient http)
    {
        _http = http;
    }

    // Get all classes
    public async Task<List<Course>> GetClassesAsync()
    {
        var response = await _http.GetAsync($"{BaseUrl}/Classes?select=*");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<Course>>(json) ?? new();
    }

    // Get a single class by ID
    public async Task<Course?> GetClassByIdAsync(long id)
    {
        var response = await _http.GetAsync($"{BaseUrl}/Classes?id=eq.{id}&select=*");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var results = JsonSerializer.Deserialize<List<Course>>(json);
        return results?.FirstOrDefault();
    }

    // Get all classes for a specific student via StudentCourses join
    public async Task<List<Course>> GetClassesForStudentAsync(long studentId)
    {
        var response = await _http.GetAsync(
            $"{BaseUrl}/StudentCourses?StudentID=eq.{studentId}&select=Classes(*)"
        );
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var results = JsonSerializer.Deserialize<List<StudentCourseRow>>(json) ?? new();
        return results
            .Select(r => r.Classes)
            .Where(c => c != null)
            .ToList()!;
    }

    // Add a class
    public async Task<bool> AddClassAsync(Course newClass)
    {
        var body = JsonContent.Create(newClass);
        var response = await _http.PostAsync($"{BaseUrl}/Classes", body);
        return response.IsSuccessStatusCode;
    }

    // Enroll a student in a class
    public async Task<bool> EnrollStudentAsync(long studentId, long courseId)
    {
        var body = JsonContent.Create(
            new StudentCourseInsert
            {
                StudentID = studentId,
                CourseID = courseId
            });
        var response = await _http.PostAsync($"{BaseUrl}/StudentCourses", body);
        return response.IsSuccessStatusCode;
    }

    // Update a class
    public async Task<bool> UpdateClassAsync(long id, Course updated)
    {
        var body = JsonContent.Create(updated);
        var request = new HttpRequestMessage(HttpMethod.Patch, $"{BaseUrl}/Classes?id=eq.{id}")
        {
            Content = body
        };
        request.Headers.Add("Prefer", "return=representation");

        var response = await _http.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    // Remove a student from a class
    public async Task<bool> UnenrollStudentAsync(long studentId, long courseId)
    {
        var response = await _http.DeleteAsync(
            $"{BaseUrl}/StudentCourses?StudentID=eq.{studentId}&CourseID=eq.{courseId}"
        );
        return response.IsSuccessStatusCode;
    }

    // Delete a class
    public async Task<bool> DeleteClassAsync(long id)
    {
        var response = await _http.DeleteAsync($"{BaseUrl}/Classes?id=eq.{id}");
        return response.IsSuccessStatusCode;
    }
}

public class StudentCourseInsert
{
    [JsonPropertyName("StudentID")]
    public long StudentID { get; set; }

    [JsonPropertyName("CourseID")]
    public long CourseID { get; set; }
}
// Wrapper to unwrap Supabase's nested join response
public class StudentCourseRow
{
    [JsonPropertyName("Classes")]
    public Course Classes { get; set; } = new();
}