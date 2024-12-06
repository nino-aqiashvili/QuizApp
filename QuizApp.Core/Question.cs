using System.Collections.Generic;

public class Question
{
    public string Text { get; set; }
    public List<string> Options { get; set; } = new();
    public int CorrectOptionIndex { get; set; }
}
