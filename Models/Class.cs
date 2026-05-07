using System.Text.Json.Serialization;

namespace CourseSelectionGuide.Models
{
    public class Course
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("SubjectName")]
        public string? SubjectName { get; set; }

        [JsonPropertyName("SubjectDescription")]
        public string? SubjectDescription { get; set; }

        [JsonPropertyName("Credits")]
        public float Credits { get; set; }

        [JsonPropertyName("Limitations")]
        public string? Limitations { get; set; }

        [JsonPropertyName("CreditType")]
        public string? CreditType { get; set; }
    }
}