using System.CommandLine;
using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;

namespace Bison
{
    public sealed class UserInterface
    {
        const string CSV_FILE_PATH = "SimpleDB/bison_observe_cli_db.csv";

        private UserInterface()
        {
        }

        public static void PrintObservations<T>(IEnumerable<T> observations)
        {
            foreach (var observation in observations)
            {
                Console.WriteLine(observation);
            }
        }

        public static RootCommand GetRootCommand()
        {
            RootCommand root = new("Bison.CLI app");

            root.Subcommands.Add(ReadCommand());
            root.Subcommands.Add(ObservationCommand());

            return root;
        }

        static Command ReadCommand()
        {
            Command read = new("read", "read the saved observations");
            read.SetAction(result =>
            {
                using var reader = new StreamReader(CSV_FILE_PATH);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var records = csv.GetRecords<ObservationRecord>();
                PrintObservations(records);
            });
            return read;
        }

        static Command ObservationCommand()
        {
            Command observe = new("observe", "adds an observation to the database");
            Argument<string> obsArg = new("observation")
            {
                Description = "the observation you observed"
            };
            observe.Arguments.Add(obsArg);
            observe.SetAction(result =>
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
            });

            return observe;
        }
    }
}
