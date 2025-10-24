using Microsoft.Data.Sqlite;

namespace DrAccessibility.App.Data;

public static class DatabaseInitializer
{
    public static void Initialize(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
        CREATE TABLE IF NOT EXISTS Patients (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            FullName TEXT NOT NULL,
            BirthDate TEXT NULL,
            Gender TEXT NOT NULL,
            DocumentId TEXT NOT NULL,
            ContactInfo TEXT NOT NULL,
            Address TEXT NOT NULL,
            Notes TEXT NOT NULL
        );

        CREATE TABLE IF NOT EXISTS Consultations (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            PatientId INTEGER NOT NULL,
            ScheduledDate TEXT NOT NULL,
            Notes TEXT NOT NULL,
            Anamnesis TEXT NOT NULL,
            FOREIGN KEY(PatientId) REFERENCES Patients(Id) ON DELETE CASCADE
        );

        CREATE TABLE IF NOT EXISTS Prescriptions (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            PatientId INTEGER NOT NULL,
            ConsultationId INTEGER NULL,
            Title TEXT NOT NULL,
            Body TEXT NOT NULL,
            CreatedAt TEXT NOT NULL,
            FOREIGN KEY(PatientId) REFERENCES Patients(Id) ON DELETE CASCADE,
            FOREIGN KEY(ConsultationId) REFERENCES Consultations(Id) ON DELETE SET NULL
        );

        CREATE TABLE IF NOT EXISTS AnamnesisTemplates (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Content TEXT NOT NULL,
            ImportedAt TEXT NOT NULL
        );

        CREATE TABLE IF NOT EXISTS PrescriptionTemplates (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Body TEXT NOT NULL,
            CreatedAt TEXT NOT NULL
        );

        CREATE TABLE IF NOT EXISTS DoctorProfile (
            Id INTEGER PRIMARY KEY CHECK (Id = 1),
            FullName TEXT NOT NULL,
            RegistrationNumber TEXT NOT NULL,
            Specialty TEXT NOT NULL,
            ClinicAddress TEXT NOT NULL,
            ContactInfo TEXT NOT NULL
        );
        ";
        command.ExecuteNonQuery();
    }
}
