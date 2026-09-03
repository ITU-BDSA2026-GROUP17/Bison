using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace SimpleDB;
public class CSVDatabase<T> : IDatabaseRepository<T>
{
    const string CSV_FILE_PATH = "SimpleDB/bison_observe_cli_db.csv";

     public IEnumerable<T> Read(int? limit = null)
    {
        using var reader = new StreamReader(CSV_FILE_PATH);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        
        var records = csv.GetRecords<T>();
        
        foreach(var record in records){
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

        using var stream = File.Open(CSV_FILE_PATH, FileMode.Append);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, config);

        csv.WriteRecord(record);
    }
}