// See https://aka.ms/new-console-template for more information
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

        public static Int32 DateTimeToUnixTimeStamp( DateTime dateTime )
        {
            DateTime unixEpoch = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = dateTime.ToUniversalTime() - unixEpoch;
            return (Int32) Math.Floor(diff.TotalSeconds);
        }
    }

    public class ObservationRecord
    {
        public required string Author { get; set; }
        public required string Observation { get; set; }
        public Int32 Timestamp { get; set; }

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

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                // TODO; add a help with list of available commands
                Console.WriteLine("Please give a command!");
                return;
            }

            var command = args[0];

            switch (command) {
                case "read":
                    {
                        ReadFromCSV();
                        return;
                    }
                default:
                    {
                        // TODO; add a help with list of available commands
                        Console.WriteLine("Please give a valid command!");
                        return;
                    }
            }
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
