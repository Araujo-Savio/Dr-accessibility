using DrAccessibility.App.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DrAccessibility.App.Services;

public class PdfPrescriptionExporter
{
    public void Export(Prescription prescription, Patient patient, DoctorProfile doctor, string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Informe um caminho de arquivo válido", nameof(filePath));
        }

        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Column(column =>
                {
                    column.Spacing(5);
                    column.Item().Text(doctor.FullName).FontSize(18).SemiBold();
                    column.Item().Text($"CRM: {doctor.RegistrationNumber}");
                    if (!string.IsNullOrWhiteSpace(doctor.Specialty))
                    {
                        column.Item().Text($"Especialidade: {doctor.Specialty}");
                    }
                    if (!string.IsNullOrWhiteSpace(doctor.ClinicAddress))
                    {
                        column.Item().Text(doctor.ClinicAddress);
                    }
                    if (!string.IsNullOrWhiteSpace(doctor.ContactInfo))
                    {
                        column.Item().Text(doctor.ContactInfo);
                    }
                });

                page.Content().Column(column =>
                {
                    column.Spacing(15);

                    column.Item().BorderBottom(1).BorderColor(Colors.Grey.Medium);

                    column.Item().Text($"Paciente: {patient.FullName}").FontSize(16).SemiBold();
                    if (patient.BirthDate is { } birthDate)
                    {
                        var age = CalculateAge(birthDate);
                        column.Item().Text($"Data de nascimento: {birthDate:dd/MM/yyyy} (Idade: {age} anos)");
                    }
                    if (!string.IsNullOrWhiteSpace(patient.DocumentId))
                    {
                        column.Item().Text($"Documento: {patient.DocumentId}");
                    }
                    if (!string.IsNullOrWhiteSpace(patient.ContactInfo))
                    {
                        column.Item().Text($"Contato: {patient.ContactInfo}");
                    }

                    column.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(10);
                    column.Item().Text(prescription.Title).FontSize(16).SemiBold();
                    column.Item().Text(prescription.Body).LineHeight(1.4f);
                });

                page.Footer().AlignRight().Text($"Emitido em {prescription.CreatedAt:dd/MM/yyyy HH:mm}");
            });
        });

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        document.GeneratePdf(filePath);
    }

    private static int CalculateAge(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - birthDate.Year;
        if (birthDate > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}
