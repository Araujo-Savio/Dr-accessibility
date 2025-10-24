using System.Globalization;
using DrAccessibility.App.Data;
using DrAccessibility.App.Models;
using Microsoft.Data.Sqlite;

namespace DrAccessibility.App.Services;

public class ClinicDataService
{
    private readonly string _connectionString;

    public ClinicDataService(string databasePath)
    {
        _connectionString = $"Data Source={databasePath}";
        DatabaseInitializer.Initialize(_connectionString);
    }

    private SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    private static object ToDbString(string? value) =>
        value is null ? DBNull.Value : value;

    private static string GetStringOrDefault(SqliteDataReader reader, int ordinal) =>
        reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);

    private static string? ToSqlDate(DateOnly? date) => date?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    private static DateOnly? FromSqlDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return DateOnly.Parse(value, CultureInfo.InvariantCulture);
    }

    private static DateTime FromSqlDateTime(string value) => DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);

    private static string ToSqlDateTime(DateTime value) => value.ToString("O", CultureInfo.InvariantCulture);

    public int AddPatient(Patient patient)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO Patients (FullName, BirthDate, Gender, DocumentId, ContactInfo, Address, Notes)
                               VALUES ($name, $birthDate, $gender, $document, $contact, $address, $notes);
                               SELECT last_insert_rowid();";
        command.Parameters.AddWithValue("$name", patient.FullName);
        command.Parameters.AddWithValue("$birthDate", ToSqlDate(patient.BirthDate) ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("$gender", ToDbString(patient.Gender));
        command.Parameters.AddWithValue("$document", ToDbString(patient.DocumentId));
        command.Parameters.AddWithValue("$contact", ToDbString(patient.ContactInfo));
        command.Parameters.AddWithValue("$address", ToDbString(patient.Address));
        command.Parameters.AddWithValue("$notes", ToDbString(patient.Notes));

        var result = command.ExecuteScalar();
        return (int)(long)result!;
    }

    public void UpdatePatient(Patient patient)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"UPDATE Patients
                               SET FullName = $name,
                                   BirthDate = $birthDate,
                                   Gender = $gender,
                                   DocumentId = $document,
                                   ContactInfo = $contact,
                                   Address = $address,
                                   Notes = $notes
                               WHERE Id = $id";
        command.Parameters.AddWithValue("$name", patient.FullName);
        command.Parameters.AddWithValue("$birthDate", ToSqlDate(patient.BirthDate) ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("$gender", ToDbString(patient.Gender));
        command.Parameters.AddWithValue("$document", ToDbString(patient.DocumentId));
        command.Parameters.AddWithValue("$contact", ToDbString(patient.ContactInfo));
        command.Parameters.AddWithValue("$address", ToDbString(patient.Address));
        command.Parameters.AddWithValue("$notes", ToDbString(patient.Notes));
        command.Parameters.AddWithValue("$id", patient.Id);

        command.ExecuteNonQuery();
    }

    public void DeletePatient(int patientId)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Patients WHERE Id = $id";
        command.Parameters.AddWithValue("$id", patientId);
        command.ExecuteNonQuery();
    }

    public Patient? GetPatientById(int id)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, FullName, BirthDate, Gender, DocumentId, ContactInfo, Address, Notes FROM Patients WHERE Id = $id";
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return MapPatient(reader);
    }

    public IReadOnlyList<Patient> GetAllPatients()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, FullName, BirthDate, Gender, DocumentId, ContactInfo, Address, Notes FROM Patients ORDER BY FullName";

        using var reader = command.ExecuteReader();
        var patients = new List<Patient>();
        while (reader.Read())
        {
            patients.Add(MapPatient(reader));
        }

        return patients;
    }

    private static Patient MapPatient(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        FullName = reader.GetString(1),
        BirthDate = FromSqlDate(reader.IsDBNull(2) ? null : reader.GetString(2)),
        Gender = GetStringOrDefault(reader, 3),
        DocumentId = GetStringOrDefault(reader, 4),
        ContactInfo = GetStringOrDefault(reader, 5),
        Address = GetStringOrDefault(reader, 6),
        Notes = GetStringOrDefault(reader, 7)
    };

    public int ScheduleConsultation(Consultation consultation)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO Consultations (PatientId, ScheduledDate, Notes, Anamnesis)
                               VALUES ($patientId, $date, $notes, $anamnesis);
                               SELECT last_insert_rowid();";
        command.Parameters.AddWithValue("$patientId", consultation.PatientId);
        command.Parameters.AddWithValue("$date", ToSqlDateTime(consultation.ScheduledDate));
        command.Parameters.AddWithValue("$notes", ToDbString(consultation.Notes));
        command.Parameters.AddWithValue("$anamnesis", ToDbString(consultation.Anamnesis));

        var result = command.ExecuteScalar();
        return (int)(long)result!;
    }

    public void UpdateConsultation(Consultation consultation)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"UPDATE Consultations
                               SET ScheduledDate = $date,
                                   Notes = $notes,
                                   Anamnesis = $anamnesis
                               WHERE Id = $id";
        command.Parameters.AddWithValue("$date", ToSqlDateTime(consultation.ScheduledDate));
        command.Parameters.AddWithValue("$notes", ToDbString(consultation.Notes));
        command.Parameters.AddWithValue("$anamnesis", ToDbString(consultation.Anamnesis));
        command.Parameters.AddWithValue("$id", consultation.Id);
        command.ExecuteNonQuery();
    }

    public void DeleteConsultation(int consultationId)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Consultations WHERE Id = $id";
        command.Parameters.AddWithValue("$id", consultationId);
        command.ExecuteNonQuery();
    }

    public Consultation? GetConsultationById(int consultationId)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, PatientId, ScheduledDate, Notes, Anamnesis FROM Consultations WHERE Id = $id";
        command.Parameters.AddWithValue("$id", consultationId);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new Consultation
        {
            Id = reader.GetInt32(0),
            PatientId = reader.GetInt32(1),
            ScheduledDate = FromSqlDateTime(reader.GetString(2)),
            Notes = GetStringOrDefault(reader, 3),
            Anamnesis = GetStringOrDefault(reader, 4)
        };
    }

    public IReadOnlyList<Consultation> GetConsultationsForPatient(int patientId)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, PatientId, ScheduledDate, Notes, Anamnesis FROM Consultations WHERE PatientId = $patientId ORDER BY ScheduledDate DESC";
        command.Parameters.AddWithValue("$patientId", patientId);

        using var reader = command.ExecuteReader();
        var consultations = new List<Consultation>();
        while (reader.Read())
        {
            consultations.Add(new Consultation
            {
                Id = reader.GetInt32(0),
                PatientId = reader.GetInt32(1),
                ScheduledDate = FromSqlDateTime(reader.GetString(2)),
                Notes = GetStringOrDefault(reader, 3),
                Anamnesis = GetStringOrDefault(reader, 4)
            });
        }

        return consultations;
    }

    public int AddAnamnesisTemplate(AnamnesisTemplate template)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO AnamnesisTemplates (Name, Content, ImportedAt)
                               VALUES ($name, $content, $importedAt);
                               SELECT last_insert_rowid();";
        command.Parameters.AddWithValue("$name", template.Name);
        command.Parameters.AddWithValue("$content", template.Content);
        command.Parameters.AddWithValue("$importedAt", ToSqlDateTime(template.ImportedAt));

        var result = command.ExecuteScalar();
        return (int)(long)result!;
    }

    public IReadOnlyList<AnamnesisTemplate> GetAnamnesisTemplates()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Content, ImportedAt FROM AnamnesisTemplates ORDER BY ImportedAt DESC";

        using var reader = command.ExecuteReader();
        var templates = new List<AnamnesisTemplate>();
        while (reader.Read())
        {
            templates.Add(new AnamnesisTemplate
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Content = reader.GetString(2),
                ImportedAt = FromSqlDateTime(reader.GetString(3))
            });
        }

        return templates;
    }

    public int AddPrescriptionTemplate(PrescriptionTemplate template)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO PrescriptionTemplates (Name, Body, CreatedAt)
                               VALUES ($name, $body, $createdAt);
                               SELECT last_insert_rowid();";
        command.Parameters.AddWithValue("$name", template.Name);
        command.Parameters.AddWithValue("$body", template.Body);
        command.Parameters.AddWithValue("$createdAt", ToSqlDateTime(template.CreatedAt));

        var result = command.ExecuteScalar();
        return (int)(long)result!;
    }

    public IReadOnlyList<PrescriptionTemplate> GetPrescriptionTemplates()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Body, CreatedAt FROM PrescriptionTemplates ORDER BY CreatedAt DESC";

        using var reader = command.ExecuteReader();
        var templates = new List<PrescriptionTemplate>();
        while (reader.Read())
        {
            templates.Add(new PrescriptionTemplate
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Body = reader.GetString(2),
                CreatedAt = FromSqlDateTime(reader.GetString(3))
            });
        }

        return templates;
    }

    public PrescriptionTemplate? GetPrescriptionTemplateById(int templateId)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Body, CreatedAt FROM PrescriptionTemplates WHERE Id = $id";
        command.Parameters.AddWithValue("$id", templateId);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new PrescriptionTemplate
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Body = reader.GetString(2),
            CreatedAt = FromSqlDateTime(reader.GetString(3))
        };
    }

    public int AddPrescription(Prescription prescription)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO Prescriptions (PatientId, ConsultationId, Title, Body, CreatedAt)
                               VALUES ($patientId, $consultationId, $title, $body, $createdAt);
                               SELECT last_insert_rowid();";
        command.Parameters.AddWithValue("$patientId", prescription.PatientId);
        command.Parameters.AddWithValue("$consultationId", prescription.ConsultationId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("$title", prescription.Title);
        command.Parameters.AddWithValue("$body", prescription.Body);
        command.Parameters.AddWithValue("$createdAt", ToSqlDateTime(prescription.CreatedAt));

        var result = command.ExecuteScalar();
        return (int)(long)result!;
    }

    public IReadOnlyList<Prescription> GetPrescriptionsForPatient(int patientId)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, PatientId, ConsultationId, Title, Body, CreatedAt FROM Prescriptions WHERE PatientId = $patientId ORDER BY CreatedAt DESC";
        command.Parameters.AddWithValue("$patientId", patientId);

        using var reader = command.ExecuteReader();
        var prescriptions = new List<Prescription>();
        while (reader.Read())
        {
            prescriptions.Add(new Prescription
            {
                Id = reader.GetInt32(0),
                PatientId = reader.GetInt32(1),
                ConsultationId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                Title = reader.GetString(3),
                Body = reader.GetString(4),
                CreatedAt = FromSqlDateTime(reader.GetString(5))
            });
        }

        return prescriptions;
    }

    public Prescription? GetPrescriptionById(int prescriptionId)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, PatientId, ConsultationId, Title, Body, CreatedAt FROM Prescriptions WHERE Id = $id";
        command.Parameters.AddWithValue("$id", prescriptionId);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new Prescription
        {
            Id = reader.GetInt32(0),
            PatientId = reader.GetInt32(1),
            ConsultationId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
            Title = reader.GetString(3),
            Body = reader.GetString(4),
            CreatedAt = FromSqlDateTime(reader.GetString(5))
        };
    }

    public void DeletePrescription(int prescriptionId)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Prescriptions WHERE Id = $id";
        command.Parameters.AddWithValue("$id", prescriptionId);
        command.ExecuteNonQuery();
    }

    public DoctorProfile GetDoctorProfile()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, FullName, RegistrationNumber, Specialty, ClinicAddress, ContactInfo FROM DoctorProfile LIMIT 1";

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new DoctorProfile
            {
                Id = reader.GetInt32(0),
                FullName = reader.GetString(1),
                RegistrationNumber = reader.GetString(2),
                Specialty = GetStringOrDefault(reader, 3),
                ClinicAddress = GetStringOrDefault(reader, 4),
                ContactInfo = GetStringOrDefault(reader, 5)
            };
        }

        return new DoctorProfile { Id = 1 };
    }

    public void UpsertDoctorProfile(DoctorProfile profile)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO DoctorProfile (Id, FullName, RegistrationNumber, Specialty, ClinicAddress, ContactInfo)
                               VALUES (1, $name, $registration, $specialty, $address, $contact)
                               ON CONFLICT(Id) DO UPDATE SET
                                   FullName = excluded.FullName,
                                   RegistrationNumber = excluded.RegistrationNumber,
                                   Specialty = excluded.Specialty,
                                   ClinicAddress = excluded.ClinicAddress,
                                   ContactInfo = excluded.ContactInfo;";
        command.Parameters.AddWithValue("$name", profile.FullName);
        command.Parameters.AddWithValue("$registration", profile.RegistrationNumber);
        command.Parameters.AddWithValue("$specialty", ToDbString(profile.Specialty));
        command.Parameters.AddWithValue("$address", ToDbString(profile.ClinicAddress));
        command.Parameters.AddWithValue("$contact", ToDbString(profile.ContactInfo));
        command.ExecuteNonQuery();
    }
}
