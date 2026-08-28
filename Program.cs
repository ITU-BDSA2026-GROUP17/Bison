// See https://aka.ms/new-console-template for more information
using System.CommandLine;
using System.Globalization;
using CsvHelper;

namespace Bison {
    public class Utilities {
        public static DateTime UnixTimeStampToDateTime(Int32 unixTimeStamp)
        {
            // taken from https://stackoverflow.com/questions/249760/how-can-i-convert-a-unix-timestamp-to-datetime-and-vice-versa
            DateTime unixEpoch = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            unixEpoch = unixEpoch.AddSeconds(unixTimeStamp).ToLocalTime();
            return unixEpoch;
        }

        public static int DateTimeToUnixTimeStamp( DateTime dateTime )
        {
            DateTime unixEpoch = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = dateTime.ToUniversalTime() - unixEpoch;
            return (int) Math.Floor(diff.TotalSeconds);
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

        public override string ToString() {
            return Author + " @ " + GetAsDateTime().ToString() + ": " + Observation;
        }
    }
    public class Program {
        const string CSV_FILE_PATH = "bison_observe_cli_db.csv";

        static RootCommand GetRootCommand() {
            RootCommand root = new("Bison.CLI app");

            Command read = new("read", "read the saved observations");
            read.SetAction(result =>
            {
                ReadFromCSV();
            });
            root.Subcommands.Add(read);

            return root;
        }

        static int Main(string[] args)
        {
            var root = GetRootCommand();

            var result = root.Parse(args);
            return result.Invoke();
        }

        static void ReadFromCSV() {
            using var reader = new StreamReader(CSV_FILE_PATH);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<ObservationRecord>();
            foreach (var record in records)
            {
                Console.WriteLine(record);
            }
        }
    }
}
