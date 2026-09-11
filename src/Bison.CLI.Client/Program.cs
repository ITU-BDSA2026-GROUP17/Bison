// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;

using Bison.Database;
using Bison.Models;
using Bison.Utilities;

#nullable enable

namespace Bison.CLI.Client
{
    public class Program
    {
        public static readonly CSVDatabase<ObservationRecord> ObservationDB = new("data/bison_observation_db.csv");
        public static readonly CSVDatabase<CommentRecord> CommentDB = new("data/bison_comment_db.csv");
        public static readonly SimpleCounter ObservationIdCounter = new("data/observation_id.txt");

        static int Main(string[] args)
        {
            var root = UserInterface.GetRootCommand();

            var result = root.Parse(args);
            return result.Invoke();
        }

        public static string? ReadObservations()
        {
            try
            {
                UserInterface.PrintCheeps(ObservationDB.Read());
                return null;
            }
            catch
            {
                return "Could not find any observations.";
            }
        }

        public static string StoreObservation(string observation)
        {
            ObservationDB.Store(new ObservationRecord
            {
                Id = ObservationIdCounter.NextNumber(),
                Author = Environment.UserName,
                Observation = observation,
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            });
            return "Observation has been saved.";
        }

        public static string TryComment(int observationId, string comment)
        {
            if (GetObservationById(observationId, ObservationDB.Read()) is not null)
            {
                CommentDB.Store(new CommentRecord
                {
                    ObservationId = observationId,
                    Author = Environment.UserName,
                    Comment = comment,
                    Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
                }
                );
                return "Comment has been saved.";
            }
            else
            {
                return string.Format("Observation id {0} does not exist", observationId);
            }
        }

        public static string? ReadComments(int observationId)
        {
            try
            {
                var obs = GetObservationById(observationId, ObservationDB.Read());
                if (obs is not null)
                {
                    Console.WriteLine(obs);
                    Console.WriteLine();
                    UserInterface.PrintCheeps(FilterComments(observationId, CommentDB.Read()));
                    return null;
                }
                else
                {
                    return "Observation does not exist.";
                }
            }
            catch
            {
                return "Could not find any comments.";
            }
        }

        public static ObservationRecord? GetObservationById(int id, IEnumerable<ObservationRecord> observations)
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

        public static IEnumerable<CommentRecord> FilterComments(int observationId, IEnumerable<CommentRecord> comments)
        {
            foreach (var comment in comments)
            {
                if (comment.ObservationId == observationId)
                {
                    yield return comment;
                }
            }
        }
    }
}
