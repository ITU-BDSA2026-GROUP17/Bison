using System.CommandLine;
using System.Globalization;

using Bison.Models;
using Bison.SimpleDB;

namespace Bison
{
    public sealed class UserInterface
    {
        static readonly CSVDatabase<ObservationRecord> ObservationDB = new("data/bison_observation_db.csv");
        static readonly CSVDatabase<CommentRecord> CommentDB = new("data/bison_comment_db.csv");
        static readonly SimpleCounter ObservationIdCounter = new("data/observation_id.txt");

        private UserInterface()
        {
        }

        public static void PrintCheeps<T>(IEnumerable<T> cheeps)
        {
            if (!cheeps.Any())
            {
                throw new("Could not find any cheeps");
            }
            else
            {
                foreach (var cheep in cheeps)
                {
                    Console.WriteLine(cheep);
                }
            }
        }

        public static RootCommand GetRootCommand()
        {
            RootCommand root = new("Bison.CLI app");

            root.Subcommands.Add(ReadCommand());
            root.Subcommands.Add(ObservationCommand());
            root.Subcommands.Add(CommentCommand());
            root.Subcommands.Add(DiscussionCommand());

            return root;
        }

        static Command ReadCommand()
        {
            Command read = new("read", "read the saved observations");
            read.SetAction(result => {
                try
                {
                    PrintCheeps(ObservationDB.Read());
                } catch {
                    Console.WriteLine("Could not find any observations.");
                }
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
            observe.SetAction(result => ObservationDB.Store(new ObservationRecord
            {
                Id = ObservationIdCounter.NextNumber(),
                Author = Environment.UserName,
                Observation = result.GetRequiredValue(obsArg),
                Timestamp = Utilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
            ));

            return observe;
        }

        static Command CommentCommand() {
            Command comment = new("comment", "adds a comment to the specified observation in the database");
            Argument<string> commentArg = new("comment")
            {
                Description = "the comment to add"
            };
            Argument<int> obsId = new("observation-id")
            {
                Description = "the id of the observation you want to comment on"
            };
            comment.Arguments.Add(obsId);
            comment.Arguments.Add(commentArg);
            comment.SetAction(result => CommentDB.Store(new CommentRecord
            {
                ObservationId = result.GetRequiredValue(obsId),
                Author = Environment.UserName,
                Comment = result.GetRequiredValue(commentArg),
                Timestamp = Utilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
            ));

            return comment;
        }

        static Command DiscussionCommand() {
            Command discussion = new("discussion", "read comments made on an observation");
            Argument<int> obsId = new("observation-id")
            {
                Description = "the id of the observation you want to read comments about"
            };
            discussion.Arguments.Add(obsId);
            discussion.SetAction(result => {
                try {
                    PrintCheeps(FilterComments(result.GetRequiredValue(obsId), CommentDB.Read()));
                } catch {
                    Console.WriteLine("Could not find any comments.");
                }
            });

            return discussion;
        }

        static IEnumerable<CommentRecord> FilterComments(int id, IEnumerable<CommentRecord> comments) {
            foreach (var comment in comments) {
                if (comment.ObservationId == id) {
                    yield return comment;
                }
            }
        }
    }
}
