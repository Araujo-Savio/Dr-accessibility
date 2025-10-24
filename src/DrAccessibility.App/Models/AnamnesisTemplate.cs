namespace DrAccessibility.App.Models;

public class AnamnesisTemplate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime ImportedAt { get; set; }
}
