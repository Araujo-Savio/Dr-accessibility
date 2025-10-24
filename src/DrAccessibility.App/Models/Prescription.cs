namespace DrAccessibility.App.Models;

public class Prescription
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int? ConsultationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
