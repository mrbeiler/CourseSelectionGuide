// Models/Users.cs
using System.Text.Json.Serialization;

namespace CourseSelectionGuide.Models
{
    public class Users
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("Grade")]
        public string Grade { get; set; } = "";

        [JsonPropertyName("Email")]
        public string Email { get; set; } = "";

        [JsonPropertyName("Password")]
        public string Password { get; set; } = "";
    }
}