using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;

#nullable enable

namespace Bison.CLI.Client
{
    public sealed class UserInterface
    {
        public static RootCommand GetRootCommand()
        {
            RootCommand root = new("Bison.CLI app");

            root.Subcommands.Add(ReadCommand());
            root.Subcommands.Add(ObservationCommand());
            root.Subcommands.Add(CommentCommand());
            root.Subcommands.Add(DiscussionCommand());
            root.Subcommands.Add(ProposeCommand());

            return root;
        }

        static Command ReadCommand()
        {
            Command read = new("read", "read the saved observations");
            read.SetAction(async result =>
            {
                var res = await Program.ReadObservations();
                if (res is not null)
                {
                    Console.WriteLine(res);
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
            Argument<string> locationArg = new("location")
            {
                Description = "location of the observation you observed"
            };
            observe.Arguments.Add(obsArg);
            observe.Arguments.Add(locationArg);
            observe.SetAction(async result => Console.WriteLine(
                await Program.StoreObservation(result.GetRequiredValue(obsArg), result.GetRequiredValue(locationArg))
            ));

            return observe;
        }

        static Command CommentCommand()
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
            comment.SetAction(async result =>
                Console.WriteLine(await Program.TryComment(
                    result.GetRequiredValue(obsIdArg),
                    result.GetRequiredValue(commentArg)
                ))
            );

            return comment;
        }

        static Command DiscussionCommand()
        {
            Command discussion = new("discussion", "read comments made on an observation");
            Argument<int> obsIdArg = new("observation-id")
            {
                Description = "the id of the observation you want to read comments about"
            };
            discussion.Arguments.Add(obsIdArg);
            discussion.SetAction(async result =>
            {
                var res = await Program.ReadComments(result.GetRequiredValue(obsIdArg));
                if (res is not null)
                {
                    Console.WriteLine(res);
                }
            });

            return discussion;
        }

        static Command ProposeCommand()
        {
            Command propose = new("propose", "propose a taxon to the specified observation in the database");
            Argument<string> proposeArg = new("taxonID")
            {
                Description = "the ID of the taxon to propose"
            };
            Argument<int> obsIdArg = new("observation-id")
            {
                Description = "the id of the observation you want to comment on"
            };
            propose.Arguments.Add(obsIdArg);
            propose.Arguments.Add(proposeArg);
            propose.SetAction(async result =>
                Console.WriteLine(await Program.TryPropose(
                    result.GetRequiredValue(obsIdArg),
                    result.GetRequiredValue(proposeArg)
                ))
            );

            return propose;
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
    }
}
