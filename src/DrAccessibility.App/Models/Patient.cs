namespace DrAccessibility.App.Models;

public class Patient
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateOnly? BirthDate { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string DocumentId { get; set; } = string.Empty;
    public string ContactInfo { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
