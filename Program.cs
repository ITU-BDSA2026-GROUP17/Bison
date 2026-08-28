// See https://aka.ms/new-console-template for more information
using System.CommandLine;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace Bison
{
    public class Utilities
    {
        public static DateTime UnixTimeStampToDateTime(Int32 unixTimeStamp)
        {
            // taken from https://stackoverflow.com/questions/249760/how-can-i-convert-a-unix-timestamp-to-datetime-and-vice-versa
            DateTime unixEpoch = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            unixEpoch = unixEpoch.AddSeconds(unixTimeStamp).ToLocalTime();
            return unixEpoch;
        }

        public static int DateTimeToUnixTimeStamp(DateTime dateTime)
        {
            DateTime unixEpoch = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = dateTime.ToUniversalTime() - unixEpoch;
            return (int)Math.Floor(diff.TotalSeconds);
        }
    }

    public class ObservationRecord
    {
        public required string Author { get; set; }
        public required string Observation { get; set; }
        public int Timestamp { get; set; }

        public DateTime GetAsDateTime()
        {
            return Utilities.UnixTimeStampToDateTime(Timestamp);
        }

        public override string ToString()
        {
            return Author + " @ " + GetAsDateTime().ToString() + ": " + Observation;
        }
    }

    public class Program
    {
        const string CSV_FILE_PATH = "bison_observe_cli_db.csv";

        static int Main(string[] args)
        {
            var root = GetRootCommand();

            var result = root.Parse(args);
            return result.Invoke();
        }

        static RootCommand GetRootCommand()
        {
            RootCommand root = new("Bison.CLI app");

            Command read = new("read", "read the saved observations");
            read.SetAction(result =>
            {
                ReadFromCSV();
            });
            root.Subcommands.Add(read);

            Command observe = new("observe", "adds an observation to the database");
            Argument<string> observation = new("observation")
            {
                Description = "the observation you observed"
            };
            observe.Arguments.Add(observation);
            observe.SetAction(result => AddObservation(result, observation));
            root.Subcommands.Add(observe);

            return root;
        }

        static void ReadFromCSV()
        {
            using var reader = new StreamReader(CSV_FILE_PATH);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<ObservationRecord>();
            foreach (var record in records)
            {
                Console.WriteLine(record);
            }
        }

        static void AddObservation(ParseResult result, Argument<string> obsArg)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Don't write the header again.
                HasHeaderRecord = false,
            };
            using var stream = File.Open(CSV_FILE_PATH, FileMode.Append);
            using var writer = new StreamWriter(stream);
            using var csv = new CsvWriter(writer, config);

            csv.WriteRecords([new ObservationRecord
                {
                    Author = Environment.UserName,
                    Observation = result.GetRequiredValue(obsArg),
                    Timestamp = Utilities.DateTimeToUnixTimeStamp(DateTime.Now),
                }
            ]);
        }
    }
}
