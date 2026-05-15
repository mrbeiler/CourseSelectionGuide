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

    // Enroll a student in a class with preference
    public async Task<bool> EnrollStudentWithPreferenceAsync(long studentId, long courseId, int? preference)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null
        };

        var data = new
        {
            StudentID = studentId,
            CourseID = courseId,
            Preference = preference
        };

        var jsonString = JsonSerializer.Serialize(data, jsonOptions);
        var body = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");

        var response = await _http.PostAsync($"{BaseUrl}/StudentCourses", body);
        return response.IsSuccessStatusCode;
    }

    // Update preference for a student's enrolled course
    public async Task<bool> UpdateCoursePreferenceAsync(long studentId, long courseId, int? preference)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null
        };

        var data = new
        {
            Preference = preference
        };

        var jsonString = JsonSerializer.Serialize(data, jsonOptions);
        var body = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Patch, 
            $"{BaseUrl}/StudentCourses?StudentID=eq.{studentId}&CourseID=eq.{courseId}")
        {
            Content = body
        };

        var response = await _http.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    // Get all enrollments for a student with preferences
    public async Task<List<StudentCourseWithPreference>> GetStudentEnrollmentsWithPreferencesAsync(long studentId)
    {
        var response = await _http.GetAsync(
            $"{BaseUrl}/StudentCourses?StudentID=eq.{studentId}&select=CourseID,Preference,Classes(*)"
        );
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<StudentCourseWithPreference>>(json) ?? new();
    }

    // Batch update preferences for multiple courses
    public async Task<bool> BatchUpdatePreferencesAsync(long studentId, Dictionary<long, int?> coursePreferences)
    {
        // Update each course preference individually
        foreach (var kvp in coursePreferences)
        {
            var success = await UpdateCoursePreferenceAsync(studentId, kvp.Key, kvp.Value);
            if (!success)
                return false;
        }
        return true;
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

    // Get all students enrolled in a specific course
    public async Task<List<StudentEnrollment>> GetStudentsForCourseAsync(long courseId)
    {
        var response = await _http.GetAsync(
            $"{BaseUrl}/StudentCourses?CourseID=eq.{courseId}&select=StudentID,Preference,Users(*)"
        );

        if (!response.IsSuccessStatusCode)
            return new List<StudentEnrollment>();

        var json = await response.Content.ReadAsStringAsync();
        var results = JsonSerializer.Deserialize<List<StudentEnrollmentRow>>(json) ?? new();

        return results.Select(r => new StudentEnrollment
        {
            StudentID = r.StudentID,
            Preference = r.Preference,
            Student = r.Users
        }).ToList();
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

// Model for StudentCourses with preference
public class StudentCourseWithPreference
{
    [JsonPropertyName("CourseID")]
    public long CourseID { get; set; }

    [JsonPropertyName("Preference")]
    public int? Preference { get; set; }

    [JsonPropertyName("Classes")]
    public Course Classes { get; set; } = new();
}

// Model for student enrollment with user details
public class StudentEnrollmentRow
{
    [JsonPropertyName("StudentID")]
    public long StudentID { get; set; }

    [JsonPropertyName("Preference")]
    public int? Preference { get; set; }

    [JsonPropertyName("Users")]
    public Users Users { get; set; } = new();
}

public class StudentEnrollment
{
    public long StudentID { get; set; }
    public int? Preference { get; set; }
    public Users Student { get; set; } = new();
}
