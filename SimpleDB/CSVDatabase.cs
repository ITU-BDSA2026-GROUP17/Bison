using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;

namespace Bison.SimpleDB;

public sealed class CSVDatabase<T>(string filePath) : IDatabaseRepository<T>
{
    readonly string _filePath = filePath;

    public IEnumerable<T> Read(int? limit = null)
    {
        using var reader = new StreamReader(_filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<T>();

        foreach (var record in records)
        {
            yield return record;
        }
    }
    public void Store(T record)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            // Don't write the header again.
            HasHeaderRecord = false,
        };
        if (!File.Exists(_filePath))
        {
            config = new CsvConfiguration(CultureInfo.InvariantCulture);
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
        }
        using var stream = File.Open(_filePath, FileMode.Append);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, config);

        csv.WriteRecords([record]);
    }
}
