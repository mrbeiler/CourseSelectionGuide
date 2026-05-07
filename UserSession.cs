// UserSession.cs

using System.Text.Json;

public class UserSession
{
    public long UserId { get; set; }
    public string Name { get; set; } = "";
    public string Grade { get; set; } = "";

    public bool IsLoggedIn => UserId > 0;

    // Used for restoring state
    public void Load(UserSessionDto dto)
    {
        UserId = dto.UserId;
        Name = dto.Name;
        Grade = dto.Grade;
    }

    public UserSessionDto ToDto()
    {
        return new UserSessionDto
        {
            UserId = UserId,
            Name = Name,
            Grade = Grade
        };
    }

    public void Clear()
    {
        UserId = 0;
        Name = "";
        Grade = "";
    }
}

public class UserSessionDto
{
    public long UserId { get; set; }
    public string Name { get; set; } = "";
    public string Grade { get; set; } = "";
}