using System.Globalization;

using Bison.Models;

using CsvHelper;
using CsvHelper.Configuration;

namespace Bison.Database;

public sealed class CSVDatabase : IDatabaseService
{
    readonly string _observationFilePath;
    readonly string _commentFilePath;
    readonly string _proposalFilePath;

    readonly SimpleCounter _observationIDCounter;
    readonly Taxonomies _taxonomies = new();

    public CSVDatabase(string observationFilePath, string commentFilePath, string observationIDCounter, string proposalFilePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(observationFilePath);
        ArgumentException.ThrowIfNullOrEmpty(commentFilePath);
        ArgumentException.ThrowIfNullOrEmpty(observationIDCounter);
        ArgumentException.ThrowIfNullOrEmpty(proposalFilePath);

        _observationFilePath = observationFilePath;
        _commentFilePath = commentFilePath;
        _observationIDCounter = new(observationIDCounter);
        _proposalFilePath = proposalFilePath;
    }

#nullable enable
    private static ObservationRecord? GetObservationById(int id, IEnumerable<ObservationRecord> observations)
    {
        foreach (var observation in observations)
        {
            if (observation.Id == id)
            {
                return observation;
            }
        }
        return null;
    }
#nullable restore

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
        if (GetObservationById(record.ObservationId, ReadObservations()) is null)
        {
            throw new ObservationDoesNotExist(record.ObservationId);
        }
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

    public IEnumerable<ProposalRecord> ReadProposalsForObservation(int observationId, int? limit = null)
    {
        if (!File.Exists(_proposalFilePath) || new FileInfo(_proposalFilePath).Length == 0)
        {
            yield break;
        }
        using var reader = new StreamReader(_proposalFilePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<ProposalRecord>();

        foreach (var record in records)
        {
            if (record.ObservationId == observationId)
            {
                yield return record;
            }
        }
    }
    public void StoreProposal(ProposalRecord record)
    {
        if (GetObservationById(record.ObservationId, ReadObservations()) is null)
        {
            throw new ObservationDoesNotExist(record.ObservationId);
        }
        if (_taxonomies.GetTaxonRecordByID(record.TaxonID) is null)
        {
            throw new TaxonDoesNotExist(record.TaxonID);
        }
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            // Don't write the header again.
            HasHeaderRecord = false,
        };
        if (!File.Exists(_proposalFilePath) || new FileInfo(_proposalFilePath).Length == 0)
        {
            config = new CsvConfiguration(CultureInfo.InvariantCulture);
            Directory.CreateDirectory(Path.GetDirectoryName(_proposalFilePath));
        }

        using var stream = File.Open(_proposalFilePath, FileMode.Append);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, config);

        csv.WriteRecords([record]);
    }
}

public class ObservationDoesNotExist(int id) : ArgumentException(string.Format("Observation with id {0} does not exist", id))
{
    public int Id = id;
}

public class TaxonDoesNotExist(string id) : ArgumentException(string.Format("Taxon with id {0} does not exist", id))
{
    public string Id = id;
}
