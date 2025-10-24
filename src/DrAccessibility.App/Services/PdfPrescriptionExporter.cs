using DrAccessibility.App.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DrAccessibility.App.Services;

public class PdfPrescriptionExporter
{
    public void Export(ExportPrescriptionInput input)
    {
        if (string.IsNullOrWhiteSpace(input.FilePath))
        {
            throw new ArgumentException("Informe um caminho de arquivo válido", nameof(ExportPrescriptionInput.FilePath));
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
                    column.Item().Text(input.Doctor.FullName).FontSize(18).SemiBold();
                    column.Item().Text($"CRM: {input.Doctor.RegistrationNumber}");
                    if (!string.IsNullOrWhiteSpace(input.Doctor.Specialty))
                    {
                        column.Item().Text($"Especialidade: {input.Doctor.Specialty}");
                    }
                    if (!string.IsNullOrWhiteSpace(input.Doctor.ClinicAddress))
                    {
                        column.Item().Text(input.Doctor.ClinicAddress);
                    }
                    if (!string.IsNullOrWhiteSpace(input.Doctor.ContactInfo))
                    {
                        column.Item().Text(input.Doctor.ContactInfo);
                    }
                });

                page.Content().Column(column =>
                {
                    column.Spacing(15);

                    column.Item().BorderBottom(1).BorderColor(Colors.Grey.Medium);

                    column.Item().Text($"Paciente: {input.Patient.FullName}").FontSize(16).SemiBold();
                    if (input.Patient.BirthDate is { } birthDate)
                    {
                        var age = CalculateAge(birthDate);
                        column.Item().Text($"Data de nascimento: {birthDate:dd/MM/yyyy} (Idade: {age} anos)");
                    }
                    if (!string.IsNullOrWhiteSpace(input.Patient.DocumentId))
                    {
                        column.Item().Text($"Documento: {input.Patient.DocumentId}");
                    }
                    if (!string.IsNullOrWhiteSpace(input.Patient.ContactInfo))
                    {
                        column.Item().Text($"Contato: {input.Patient.ContactInfo}");
                    }

                    column.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(10);
                    column.Item().Text(input.Prescription.Title).FontSize(16).SemiBold();
                    column.Item().Text(input.Prescription.Body).LineHeight(1.4f);
                });

                page.Footer().AlignRight().Text($"Emitido em {input.Prescription.CreatedAt:dd/MM/yyyy HH:mm}");
            });
        });

        var directory = Path.GetDirectoryName(input.FilePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        document.GeneratePdf(input.FilePath);
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
