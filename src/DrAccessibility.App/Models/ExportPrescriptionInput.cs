namespace DrAccessibility.App.Models;

public class ExportPrescriptionInput
{
    public required Prescription Prescription { get; init; }
    public required Patient Patient { get; init; }
    public required DoctorProfile Doctor { get; init; }
    public required string FilePath { get; init; }
}
