using System.Text.Json.Serialization;

namespace CourseSelectionGuide.Models;

public class Course
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("Name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("Description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("Department")]
    public string Department { get; set; } = "";

    [JsonPropertyName("Credits")]
    public float Credits { get; set; }

    [JsonPropertyName("GradeMin")]
    public int GradeMin { get; set; }

    [JsonPropertyName("GradeMax")]
    public int GradeMax { get; set; }

    [JsonPropertyName("IsRequired")]
    public bool IsRequired { get; set; }

    // math | science | both
    [JsonPropertyName("CreditType")]
    public string? CreditType { get; set; }

    [JsonPropertyName("Prerequisite")]
    public string? Prerequisite { get; set; }

    [JsonPropertyName("MusicChoice")]
    public bool MusicChoice { get; set; }

    [JsonPropertyName("MusicLevel")]
    public string? MusicLevel { get; set; }

    [JsonPropertyName("GenderNote")]
    public string? GenderNote { get; set; }

    [JsonPropertyName("GradePreference")]
    public int? GradePreference { get; set; }

    [JsonPropertyName("Notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("Limitations")]
    public string? Limitations { get; set; }
}