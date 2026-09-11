using System.CommandLine;
using System.Globalization;

using Bison.Database;
using Bison.Models;
using Bison.Utilities;

#nullable enable

namespace Bison.CLI.Client
{
    public sealed class UserInterface(string observationDBPath, string commentDBPath, string observationIdCounterPath)
    {
        public readonly CSVDatabase<ObservationRecord> ObservationDB = new(observationDBPath);
        public readonly CSVDatabase<CommentRecord> CommentDB = new(commentDBPath);
        public readonly SimpleCounter ObservationIdCounter = new(observationIdCounterPath);

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

        public RootCommand GetRootCommand()
        {
            RootCommand root = new("Bison.CLI app");

            root.Subcommands.Add(ReadCommand());
            root.Subcommands.Add(ObservationCommand());
            root.Subcommands.Add(CommentCommand());
            root.Subcommands.Add(DiscussionCommand());

            return root;
        }

        Command ReadCommand()
        {
            Command read = new("read", "read the saved observations");
            read.SetAction(result =>
            {
                try
                {
                    PrintCheeps(ObservationDB.Read());
                }
                catch
                {
                    Console.WriteLine("Could not find any observations.");
                }
            });
            return read;
        }

        Command ObservationCommand()
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
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
            ));

            return observe;
        }

        Command CommentCommand()
        {
            Command comment = new("comment", "adds a comment to the specified observation in the database");
            Argument<string> commentArg = new("comment")
            {
                Description = "the comment to add"
            };
            Argument<int> obsIdArg = new("observation-id")
            {
                Description = "the id of the observation you want to comment on"
            };
            comment.Arguments.Add(obsIdArg);
            comment.Arguments.Add(commentArg);
            comment.SetAction(result =>
            {
                var obsId = result.GetRequiredValue(obsIdArg);
                if (GetObservationById(obsId) is not null)
                {
                    CommentDB.Store(new CommentRecord
                    {
                        ObservationId = obsId,
                        Author = Environment.UserName,
                        Comment = result.GetRequiredValue(commentArg),
                        Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
                    }
                    );
                }
                else
                {
                    Console.WriteLine("Observation id {0} does not exist", obsId);
                }
            });

            return comment;
        }

        Command DiscussionCommand()
        {
            Command discussion = new("discussion", "read comments made on an observation");
            Argument<int> obsIdArg = new("observation-id")
            {
                Description = "the id of the observation you want to read comments about"
            };
            discussion.Arguments.Add(obsIdArg);
            discussion.SetAction(result =>
            {
                try
                {
                    var obsId = result.GetRequiredValue(obsIdArg);
                    var obs = GetObservationById(obsId);
                    if (obs is not null)
                    {
                        Console.WriteLine(obs);
                        Console.WriteLine();
                        PrintCheeps(FilterComments(obsId, CommentDB.Read()));
                    }
                    else
                    {
                        Console.WriteLine("Observation does not exist.");
                    }
                }
                catch
                {
                    Console.WriteLine("Could not find any comments.");
                }
            });

            return discussion;
        }

        ObservationRecord? GetObservationById(int id)
        {
            foreach (var observation in ObservationDB.Read())
            {
                if (observation.Id == id)
                {
                    return observation;
                }
            }
            return null;
        }

        public static IEnumerable<CommentRecord> FilterComments(int id, IEnumerable<CommentRecord> comments)
        {
            foreach (var comment in comments)
            {
                if (comment.ObservationId == id)
                {
                    yield return comment;
                }
            }
        }
    }
}
