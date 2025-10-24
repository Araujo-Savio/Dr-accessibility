using System.Text;
using DrAccessibility.App.Models;
using ExcelDataReader;

namespace DrAccessibility.App.Services;

public class ExcelImporter
{
    public AnamnesisTemplate ImportAnamnesis(string filePath, string templateName)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Arquivo não encontrado: {filePath}");
        }

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = ExcelReaderFactory.CreateReader(stream);
        var builder = new StringBuilder();

        do
        {
            while (reader.Read())
            {
                var values = new List<string>();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var value = reader.GetValue(i)?.ToString()?.Trim();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        values.Add(value);
                    }
                }

                if (values.Count > 0)
                {
                    builder.AppendLine(string.Join(" | ", values));
                }
            }
        } while (reader.NextResult());

        return new AnamnesisTemplate
        {
            Name = templateName,
            Content = builder.ToString().Trim(),
            ImportedAt = DateTime.Now
        };
    }
}
