namespace DrAccessibility.App.Models;

public class PrescriptionTemplate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
