using System;

namespace DrAccessibility.App.Models;

public class ExportPrescriptionInput
{
    public ExportPrescriptionInput(Prescription prescription, Patient patient, DoctorProfile doctor, string filePath)
    {
        Prescription = prescription ?? throw new ArgumentNullException(nameof(prescription));
        Patient = patient ?? throw new ArgumentNullException(nameof(patient));
        Doctor = doctor ?? throw new ArgumentNullException(nameof(doctor));
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public Prescription Prescription { get; }
    public Patient Patient { get; }
    public DoctorProfile Doctor { get; }
    public string FilePath { get; }
}
