using System.Globalization;
using System.Linq;
using DrAccessibility.App.Models;
using DrAccessibility.App.Services;

var databasePath = Path.Combine(AppContext.BaseDirectory, "clinic.db");
var dataService = new ClinicDataService(databasePath);
var excelImporter = new ExcelImporter();
var pdfExporter = new PdfPrescriptionExporter();

Console.OutputEncoding = System.Text.Encoding.UTF8;

ShowWelcome();

while (true)
{
    Console.WriteLine();
    Console.WriteLine("================ MENU PRINCIPAL ================");
    Console.WriteLine("1. Configurar perfil do médico");
    Console.WriteLine("2. Cadastrar paciente");
    Console.WriteLine("3. Atualizar paciente");
    Console.WriteLine("4. Listar pacientes");
    Console.WriteLine("5. Remover paciente");
    Console.WriteLine("6. Importar modelo de anamnese (Excel)");
    Console.WriteLine("7. Listar modelos de anamnese");
    Console.WriteLine("8. Agendar consulta");
    Console.WriteLine("9. Listar consultas do paciente");
    Console.WriteLine("10. Registrar receita");
    Console.WriteLine("11. Exportar receita para PDF");
    Console.WriteLine("12. Listar receitas do paciente");
    Console.WriteLine("13. Criar modelo de receita");
    Console.WriteLine("14. Listar modelos de receita");
    Console.WriteLine("15. Excluir consulta");
    Console.WriteLine("16. Excluir receita");
    Console.WriteLine("0. Sair");
    Console.Write("Escolha uma opção: ");

    var option = Console.ReadLine();
    Console.WriteLine();

    try
    {
        switch (option)
        {
            case "1":
                ConfigureDoctorProfile();
                break;
            case "2":
                RegisterPatient();
                break;
            case "3":
                UpdatePatient();
                break;
            case "4":
                ListPatients();
                break;
            case "5":
                DeletePatient();
                break;
            case "6":
                ImportAnamnesisTemplate();
                break;
            case "7":
                ListAnamnesisTemplates();
                break;
            case "8":
                ScheduleConsultation();
                break;
            case "9":
                ListConsultationsForPatient();
                break;
            case "10":
                CreatePrescription();
                break;
            case "11":
                ExportPrescriptionToPdf();
                break;
            case "12":
                ListPrescriptionsForPatient();
                break;
            case "13":
                CreatePrescriptionTemplate();
                break;
            case "14":
                ListPrescriptionTemplates();
                break;
            case "15":
                DeleteConsultation();
                break;
            case "16":
                DeletePrescription();
                break;
            case "0":
                Console.WriteLine("Até logo!");
                return;
            default:
                Console.WriteLine("Opção inválida. Tente novamente.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro: {ex.Message}");
    }
}

void ShowWelcome()
{
    Console.WriteLine("================================================");
    Console.WriteLine("         Dr. Accessibility - Modo Offline        ");
    Console.WriteLine("================================================");
    Console.WriteLine("Aplicação pensada para regiões com baixa conectividade.");
    Console.WriteLine("Todos os dados são armazenados localmente e podem ser sincronizados futuramente.");
}

void ConfigureDoctorProfile()
{
    var profile = dataService.GetDoctorProfile();
    Console.WriteLine("-- Perfil do médico --");
    Console.WriteLine($"Nome atual: {profile.FullName}");
    Console.Write("Nome completo: ");
    profile.FullName = ReadNonEmptyLine(profile.FullName);

    Console.WriteLine($"CRM atual: {profile.RegistrationNumber}");
    Console.Write("CRM/Registro profissional: ");
    profile.RegistrationNumber = ReadNonEmptyLine(profile.RegistrationNumber);

    Console.WriteLine($"Especialidade atual: {profile.Specialty}");
    Console.Write("Especialidade: ");
    profile.Specialty = ReadOptionalLine(profile.Specialty);

    Console.WriteLine($"Endereço atual: {profile.ClinicAddress}");
    Console.Write("Endereço da clínica: ");
    profile.ClinicAddress = ReadOptionalLine(profile.ClinicAddress);

    Console.WriteLine($"Contato atual: {profile.ContactInfo}");
    Console.Write("Informações de contato: ");
    profile.ContactInfo = ReadOptionalLine(profile.ContactInfo);

    dataService.UpsertDoctorProfile(profile);
    Console.WriteLine("Perfil atualizado com sucesso!");
}

void RegisterPatient()
{
    Console.WriteLine("-- Cadastro de paciente --");
    var patient = new Patient();
    Console.Write("Nome completo: ");
    patient.FullName = ReadNonEmptyLine();

    Console.Write("Data de nascimento (dd/mm/aaaa) ou deixe vazio: ");
    patient.BirthDate = ReadDateOnly();

    Console.Write("Gênero: ");
    patient.Gender = ReadOptionalLine();

    Console.Write("Documento (RG/CPF): ");
    patient.DocumentId = ReadOptionalLine();

    Console.Write("Contato: ");
    patient.ContactInfo = ReadOptionalLine();

    Console.Write("Endereço: ");
    patient.Address = ReadOptionalLine();

    Console.Write("Observações: ");
    patient.Notes = ReadOptionalLine();

    var id = dataService.AddPatient(patient);
    Console.WriteLine($"Paciente cadastrado com sucesso! ID: {id}");
}

void UpdatePatient()
{
    var patient = SelectPatient();
    if (patient is null)
    {
        return;
    }

    Console.WriteLine("-- Atualização de paciente --");
    Console.WriteLine($"Nome atual: {patient.FullName}");
    Console.Write("Novo nome (enter para manter): ");
    patient.FullName = ReadOptionalLine(patient.FullName);

    Console.WriteLine($"Data de nascimento atual: {(patient.BirthDate.HasValue ? patient.BirthDate: "não informada")}");
    Console.Write("Nova data de nascimento (dd/mm/aaaa) ou enter para manter: ");
    var newBirth = ReadDateOnly();
    if (newBirth.HasValue)
    {
        patient.BirthDate = newBirth;
    }

    Console.WriteLine($"Gênero atual: {patient.Gender}");
    Console.Write("Novo gênero: ");
    patient.Gender = ReadOptionalLine(patient.Gender);

    Console.WriteLine($"Documento atual: {patient.DocumentId}");
    Console.Write("Novo documento: ");
    patient.DocumentId = ReadOptionalLine(patient.DocumentId);

    Console.WriteLine($"Contato atual: {patient.ContactInfo}");
    Console.Write("Novo contato: ");
    patient.ContactInfo = ReadOptionalLine(patient.ContactInfo);

    Console.WriteLine($"Endereço atual: {patient.Address}");
    Console.Write("Novo endereço: ");
    patient.Address = ReadOptionalLine(patient.Address);

    Console.WriteLine($"Observações atuais: {patient.Notes}");
    Console.Write("Novas observações: ");
    patient.Notes = ReadOptionalLine(patient.Notes);

    dataService.UpdatePatient(patient);
    Console.WriteLine("Paciente atualizado!");
}

void DeletePatient()
{
    var patient = SelectPatient();
    if (patient is null)
    {
        return;
    }

    Console.Write($"Confirma remover o paciente {patient.FullName}? (s/n): ");
    var confirmation = Console.ReadLine();
    if (string.Equals(confirmation, "s", StringComparison.OrdinalIgnoreCase))
    {
        dataService.DeletePatient(patient.Id);
        Console.WriteLine("Paciente removido.");
    }
    else
    {
        Console.WriteLine("Operação cancelada.");
    }
}

void ListPatients()
{
    var patients = dataService.GetAllPatients();
    if (patients.Count == 0)
    {
        Console.WriteLine("Nenhum paciente cadastrado.");
        return;
    }

    Console.WriteLine("-- Pacientes cadastrados --");
    foreach (var patient in patients)
    {
        Console.WriteLine($"[{patient.Id}] {patient.FullName} - Contato: {patient.ContactInfo}");
    }
}

void ImportAnamnesisTemplate()
{
    Console.WriteLine("-- Importação de modelo de anamnese --");
    Console.Write("Informe o caminho do arquivo Excel: ");
    var path = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(path))
    {
        Console.WriteLine("Caminho inválido.");
        return;
    }

    Console.Write("Nome para o modelo: ");
    var name = ReadNonEmptyLine();

    var template = excelImporter.ImportAnamnesis(path, name);
    var id = dataService.AddAnamnesisTemplate(template);
    Console.WriteLine($"Modelo importado com sucesso! ID: {id}");
}

void ListAnamnesisTemplates()
{
    var templates = dataService.GetAnamnesisTemplates();
    if (templates.Count == 0)
    {
        Console.WriteLine("Nenhum modelo cadastrado.");
        return;
    }

    Console.WriteLine("-- Modelos de anamnese --");
    foreach (var template in templates)
    {
        Console.WriteLine($"[{template.Id}] {template.Name} - Importado em {template.ImportedAt:g}");
    }
}

void ScheduleConsultation()
{
    var patient = SelectPatient();
    if (patient is null)
    {
        return;
    }

    Console.WriteLine("-- Agendamento de consulta --");
    Console.Write("Data e hora (dd/mm/aaaa HH:mm): ");
    var date = ReadDateTime();
    if (date is null)
    {
        Console.WriteLine("Data inválida.");
        return;
    }

    Console.Write("Observações: ");
    var notes = ReadOptionalLine();

    var anamnesis = string.Empty;
    var templates = dataService.GetAnamnesisTemplates();
    if (templates.Count > 0)
    {
        Console.Write("Deseja usar um modelo de anamnese? (s/n): ");
        var answer = Console.ReadLine();
        if (string.Equals(answer, "s", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var template in templates)
            {
                Console.WriteLine($"[{template.Id}] {template.Name}");
            }

            Console.Write("Informe o ID do modelo: ");
            if (int.TryParse(Console.ReadLine(), out var templateId))
            {
                var selected = templates.FirstOrDefault(t => t.Id == templateId);
                if (selected is not null)
                {
                    anamnesis = selected.Content;
                }
            }
        }
    }

    if (string.IsNullOrWhiteSpace(anamnesis))
    {
        Console.Write("Descreva a anamnese: ");
        anamnesis = ReadOptionalLine();
    }

    var consultation = new Consultation
    {
        PatientId = patient.Id,
        ScheduledDate = date.Value,
        Notes = notes,
        Anamnesis = anamnesis
    };

    var id = dataService.ScheduleConsultation(consultation);
    Console.WriteLine($"Consulta agendada! ID: {id}");
}

void ListConsultationsForPatient()
{
    var patient = SelectPatient();
    if (patient is null)
    {
        return;
    }

    var consultations = dataService.GetConsultationsForPatient(patient.Id);
    if (consultations.Count == 0)
    {
        Console.WriteLine("Nenhuma consulta encontrada para este paciente.");
        return;
    }

    Console.WriteLine($"-- Consultas de {patient.FullName} --");
    foreach (var consultation in consultations)
    {
        Console.WriteLine($"[{consultation.Id}] {consultation.ScheduledDate:g} - {consultation.Notes}");
    }
}

void CreatePrescriptionTemplate()
{
    Console.WriteLine("-- Novo modelo de receita --");
    Console.Write("Nome do modelo: ");
    var name = ReadNonEmptyLine();

    Console.WriteLine("Informe o texto base da receita. Utilize marcadores como {{Paciente.Nome}} e {{Paciente.Idade}}.");
    var body = ReadNonEmptyLine();

    var template = new PrescriptionTemplate
    {
        Name = name,
        Body = body,
        CreatedAt = DateTime.Now
    };

    var id = dataService.AddPrescriptionTemplate(template);
    Console.WriteLine($"Modelo salvo! ID: {id}");
}

void ListPrescriptionTemplates()
{
    var templates = dataService.GetPrescriptionTemplates();
    if (templates.Count == 0)
    {
        Console.WriteLine("Nenhum modelo cadastrado.");
        return;
    }

    Console.WriteLine("-- Modelos de receita --");
    foreach (var template in templates)
    {
        Console.WriteLine($"[{template.Id}] {template.Name} - Criado em {template.CreatedAt:g}");
    }
}

void CreatePrescription()
{
    var patient = SelectPatient();
    if (patient is null)
    {
        return;
    }

    int? consultationId = null;
    Console.Write("Associar a uma consulta existente? (s/n): ");
    var linkConsultation = Console.ReadLine();
    if (string.Equals(linkConsultation, "s", StringComparison.OrdinalIgnoreCase))
    {
        var consultations = dataService.GetConsultationsForPatient(patient.Id);
        if (consultations.Count == 0)
        {
            Console.WriteLine("Este paciente não possui consultas.");
        }
        else
        {
            foreach (var consultation in consultations)
            {
                Console.WriteLine($"[{consultation.Id}] {consultation.ScheduledDate:g} - {consultation.Notes}");
            }

            Console.Write("Informe o ID da consulta: ");
            if (int.TryParse(Console.ReadLine(), out var selectedId) &&
                consultations.Any(c => c.Id == selectedId))
            {
                consultationId = selectedId;
            }
        }
    }

    var templates = dataService.GetPrescriptionTemplates();
    string body;
    string title;

    if (templates.Count > 0)
    {
        Console.Write("Deseja utilizar um modelo? (s/n): ");
        var answer = Console.ReadLine();
        if (string.Equals(answer, "s", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var template in templates)
            {
                Console.WriteLine($"[{template.Id}] {template.Name}");
            }

            Console.Write("Informe o ID do modelo: ");
            if (int.TryParse(Console.ReadLine(), out var templateId))
            {
                var template = dataService.GetPrescriptionTemplateById(templateId);
                if (template is not null)
                {
                    body = ApplyTemplate(template.Body, patient);
                    Console.WriteLine("Você pode complementar o texto da receita (enter para manter):");
                    var extra = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(extra))
                    {
                        body += $"\n\nObservações adicionais:\n{extra}";
                    }

                    Console.Write("Título da receita: ");
                    title = ReadOptionalLine(template.Name);
                    SavePrescription(patient, consultationId, title, body);
                    return;
                }
            }
        }
    }

    Console.Write("Título da receita: ");
    title = ReadNonEmptyLine();
    Console.WriteLine("Descrição detalhada: ");
    body = ReadNonEmptyLine();

    SavePrescription(patient, consultationId, title, body);
}

void SavePrescription(Patient patient, int? consultationId, string title, string body)
{
    var prescription = new Prescription
    {
        PatientId = patient.Id,
        ConsultationId = consultationId,
        Title = title,
        Body = body,
        CreatedAt = DateTime.Now
    };

    var id = dataService.AddPrescription(prescription);
    Console.WriteLine($"Receita cadastrada! ID: {id}");

    Console.Write("Deseja gerar o PDF agora? (s/n): ");
    var answer = Console.ReadLine();
    if (string.Equals(answer, "s", StringComparison.OrdinalIgnoreCase))
    {
        ExportPrescriptionToPdf(id);
    }
}

void ExportPrescriptionToPdf() => ExportPrescriptionToPdf(null);

void ExportPrescriptionToPdf(int? prescriptionId)
{
    int id = prescriptionId ?? ReadPrescriptionId();
    if (id == 0)
    {
        return;
    }

    var prescription = dataService.GetPrescriptionById(id);
    if (prescription is null)
    {
        Console.WriteLine("Receita não encontrada.");
        return;
    }

    var patient = dataService.GetPatientById(prescription.PatientId);
    if (patient is null)
    {
        Console.WriteLine("Paciente não encontrado para esta receita.");
        return;
    }

    var doctor = dataService.GetDoctorProfile();
    if (string.IsNullOrWhiteSpace(doctor.FullName) || string.IsNullOrWhiteSpace(doctor.RegistrationNumber))
    {
        Console.WriteLine("Complete o perfil do médico antes de gerar PDFs.");
        ConfigureDoctorProfile();
        doctor = dataService.GetDoctorProfile();
    }

    Console.Write("Informe o caminho para salvar o PDF (ex: /home/usuario/receita.pdf): ");
    var path = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(path))
    {
        Console.WriteLine("Caminho inválido.");
        return;
    }

    pdfExporter.Export(prescription, patient, doctor, path);
    Console.WriteLine("PDF gerado com sucesso!");
}

void ListPrescriptionsForPatient()
{
    var patient = SelectPatient();
    if (patient is null)
    {
        return;
    }

    var prescriptions = dataService.GetPrescriptionsForPatient(patient.Id);
    if (prescriptions.Count == 0)
    {
        Console.WriteLine("Nenhuma receita para este paciente.");
        return;
    }

    Console.WriteLine($"-- Receitas de {patient.FullName} --");
    foreach (var prescription in prescriptions)
    {
        Console.WriteLine($"[{prescription.Id}] {prescription.Title} - {prescription.CreatedAt:g}");
    }
}

void DeleteConsultation()
{
    Console.Write("Informe o ID da consulta para excluir: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("ID inválido.");
        return;
    }

    var consultation = dataService.GetConsultationById(id);
    if (consultation is null)
    {
        Console.WriteLine("Consulta não encontrada.");
        return;
    }

    Console.Write($"Confirma excluir a consulta marcada para {consultation.ScheduledDate:g}? (s/n): ");
    var answer = Console.ReadLine();
    if (string.Equals(answer, "s", StringComparison.OrdinalIgnoreCase))
    {
        dataService.DeleteConsultation(id);
        Console.WriteLine("Consulta excluída.");
    }
    else
    {
        Console.WriteLine("Operação cancelada.");
    }
}

void DeletePrescription()
{
    var id = ReadPrescriptionId();
    if (id == 0)
    {
        return;
    }

    var prescription = dataService.GetPrescriptionById(id);
    if (prescription is null)
    {
        Console.WriteLine("Receita não encontrada.");
        return;
    }

    Console.Write($"Confirma excluir a receita '{prescription.Title}'? (s/n): ");
    var answer = Console.ReadLine();
    if (string.Equals(answer, "s", StringComparison.OrdinalIgnoreCase))
    {
        dataService.DeletePrescription(id);
        Console.WriteLine("Receita excluída.");
    }
    else
    {
        Console.WriteLine("Operação cancelada.");
    }
}

int ReadPrescriptionId()
{
    Console.Write("Informe o ID da receita: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("ID inválido.");
        return 0;
    }

    return id;
}

Patient? SelectPatient()
{
    var patients = dataService.GetAllPatients();
    if (patients.Count == 0)
    {
        Console.WriteLine("Nenhum paciente cadastrado.");
        return null;
    }

    foreach (var patient in patients)
    {
        Console.WriteLine($"[{patient.Id}] {patient.FullName}");
    }

    Console.Write("Informe o ID do paciente: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("ID inválido.");
        return null;
    }

    var selected = dataService.GetPatientById(id);
    if (selected is null)
    {
        Console.WriteLine("Paciente não encontrado.");
        return null;
    }

    return selected;
}

string ReadNonEmptyLine(string? current = null)
{
    while (true)
    {
        var input = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(input))
        {
            return input.Trim();
        }

        if (!string.IsNullOrWhiteSpace(current))
        {
            return current;
        }

        Console.Write("Valor obrigatório. Informe novamente: ");
    }
}

string ReadOptionalLine(string? current = "")
{
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input))
    {
        return current ?? string.Empty;
    }

    return input.Trim();
}

DateOnly? ReadDateOnly()
{
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input))
    {
        return null;
    }

    if (DateTime.TryParseExact(input, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
    {
        return DateOnly.FromDateTime(date);
    }

    Console.WriteLine("Data inválida. Valor ignorado.");
    return null;
}

DateTime? ReadDateTime()
{
    var input = Console.ReadLine();
    if (DateTime.TryParseExact(input, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var date))
    {
        return date;
    }

    return null;
}

string ApplyTemplate(string templateBody, Patient patient)
{
    var result = templateBody.Replace("{{Paciente.Nome}}", patient.FullName, StringComparison.OrdinalIgnoreCase);

    if (patient.BirthDate.HasValue)
    {
        var age = CalculateAge(patient.BirthDate.Value);
        result = result.Replace("{{Paciente.Idade}}", age.ToString(CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase)
                       .Replace("{{Paciente.DataNascimento}}", patient.BirthDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase);
    }
    else
    {
        result = result.Replace("{{Paciente.Idade}}", "", StringComparison.OrdinalIgnoreCase)
                       .Replace("{{Paciente.DataNascimento}}", "", StringComparison.OrdinalIgnoreCase);
    }

    return result;
}

int CalculateAge(DateOnly birthDate)
{
    var today = DateOnly.FromDateTime(DateTime.Today);
    var age = today.Year - birthDate.Year;
    if (birthDate > today.AddYears(-age))
    {
        age--;
    }

    return age;
}
