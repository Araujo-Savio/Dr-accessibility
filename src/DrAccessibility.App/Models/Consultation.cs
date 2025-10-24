namespace DrAccessibility.App.Models;

public class Consultation
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string Anamnesis { get; set; } = string.Empty;
}
