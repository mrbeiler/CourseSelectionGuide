// UserSession.cs

public class UserSession
{
    public long UserId { get; set; }
    public string Name { get; set; } = "";
    public string Grade { get; set; } = "";
    public bool IsLoggedIn => UserId > 0;
}