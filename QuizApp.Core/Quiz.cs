using System.Collections.Generic;

public class Quiz
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; }
    public string CreatorUsername { get; set; }
    public List<Question> Questions { get; set; } = new();
}
