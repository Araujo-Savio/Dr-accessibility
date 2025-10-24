namespace DrAccessibility.App.Models;

public class DoctorProfile
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string ClinicAddress { get; set; } = string.Empty;
    public string ContactInfo { get; set; } = string.Empty;
}
