using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;

namespace Bison.Database;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    static readonly Dictionary<string, CSVDatabase<T>> dict = [];
    string FilePath { get; init; }

    private CSVDatabase() { }

    public static CSVDatabase<T> GetInstance(string filePath)
    {
        if (!dict.TryGetValue(filePath, out var value))
        {
            value = new()
            {
                FilePath = filePath
            };
            dict[filePath] = value;
        }
        return value;
    }

    public IEnumerable<T> Read(int? limit = null)
    {
        if (!File.Exists(FilePath))
        {
            yield break;
        }
        using var reader = new StreamReader(FilePath);
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
        if (!File.Exists(FilePath))
        {
            config = new CsvConfiguration(CultureInfo.InvariantCulture);
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
        }
        using var stream = File.Open(FilePath, FileMode.Append);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, config);

        csv.WriteRecords([record]);
    }
}
