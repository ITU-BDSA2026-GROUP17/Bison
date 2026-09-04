using System.CommandLine;
using System.Globalization;

using Bison.Models;

using SimpleDB;

namespace Bison
{
    public sealed class UserInterface
    {
        static readonly CSVDatabase<ObservationRecord> DataBase = new();

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
            read.SetAction(result => PrintObservations(DataBase.Read()));
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
            observe.SetAction(result => DataBase.Store(new ObservationRecord
            {
                Author = Environment.UserName,
                Observation = result.GetRequiredValue(obsArg),
                Timestamp = Utilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
            ));

            return observe;
        }
    }
}
