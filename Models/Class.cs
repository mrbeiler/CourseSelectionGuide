namespace CourseSelectionGuide.Models
{
    public class Course
{
    public int Id { get; set; }

    public string? SubjectName { get; set; }

    public string? SubjectDescription { get; set; }

    public float Credits { get; set; }


    public string? Limitations { get; set; }

    public string? CreditType { get; set; }
}
}
