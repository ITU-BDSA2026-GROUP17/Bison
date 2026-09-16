using System.Globalization;

using Bison.Models;

using CsvHelper;
using CsvHelper.Configuration;

namespace Bison.Database;

public sealed class CSVDatabase : IDatabaseService
{
    readonly string _observationFilePath;
    readonly string _commentFilePath;

    readonly SimpleCounter _observationIDCounter;

    public CSVDatabase(string observationFilePath, string commentFilePath, string observationIDCounter)
    {
        ArgumentException.ThrowIfNullOrEmpty(observationFilePath);
        ArgumentException.ThrowIfNullOrEmpty(commentFilePath);
        ArgumentException.ThrowIfNullOrEmpty(observationIDCounter);

        _observationFilePath = observationFilePath;
        _commentFilePath = commentFilePath;
        _observationIDCounter = new(observationIDCounter);
    }

    public IEnumerable<ObservationRecord> ReadObservations(int? limit = null)
    {
        if (!File.Exists(_observationFilePath) || new FileInfo(_observationFilePath).Length == 0)
        {
            yield break;
        }
        using var reader = new StreamReader(_observationFilePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<ObservationRecord>();

        foreach (var record in records)
        {
            yield return record;
        }
    }
    public int StoreObservation(ObservationRecord record)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            // Don't write the header again.
            HasHeaderRecord = false,
        };
        if (!File.Exists(_observationFilePath) || new FileInfo(_observationFilePath).Length == 0)
        {
            config = new CsvConfiguration(CultureInfo.InvariantCulture);
            Directory.CreateDirectory(Path.GetDirectoryName(_observationFilePath));
        }
        record.Id = _observationIDCounter.NextNumber();

        using var stream = File.Open(_observationFilePath, FileMode.Append);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, config);

        csv.WriteRecords([record]);
        return record.Id;
    }

    public IEnumerable<CommentRecord> ReadCommentsForObservation(int observationId, int? limit = null)
    {
        if (!File.Exists(_commentFilePath) || new FileInfo(_commentFilePath).Length == 0)
        {
            yield break;
        }
        using var reader = new StreamReader(_commentFilePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<CommentRecord>();

        foreach (var record in records)
        {
            if (record.ObservationId == observationId)
            {
                yield return record;
            }
        }
    }
    public void StoreComment(CommentRecord record)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            // Don't write the header again.
            HasHeaderRecord = false,
        };
        if (!File.Exists(_commentFilePath) || new FileInfo(_commentFilePath).Length == 0)
        {
            config = new CsvConfiguration(CultureInfo.InvariantCulture);
            Directory.CreateDirectory(Path.GetDirectoryName(_commentFilePath));
        }

        using var stream = File.Open(_commentFilePath, FileMode.Append);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, config);

        csv.WriteRecords([record]);
    }
}
