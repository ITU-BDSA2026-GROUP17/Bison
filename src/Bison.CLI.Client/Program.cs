// See https://aka.ms/new-console-template for more information

using System.Net.Http.Headers;
using System.Net.Http.Json;

using Bison.Models;
using Bison.Utilities;

#nullable enable

namespace Bison.CLI.Client
{
    public class Program
    {
        static readonly HttpClient DBClient = new();
        const string DATABASE_URI = "http://localhost:5001";

        public static void InitializeDBClient()
        {
            DBClient.DefaultRequestHeaders.Accept.Clear();
            DBClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            DBClient.BaseAddress = new(DATABASE_URI);
        }

        static int Main(string[] args)
        {
            InitializeDBClient();
            var root = UserInterface.GetRootCommand();

            var result = root.Parse(args);
            return result.Invoke();
        }

        public async static Task<string?> ReadObservations()
        {
            try
            {
                var comments = await DBClient.GetFromJsonAsync<List<ObservationRecord>>("/observations");
                ArgumentNullException.ThrowIfNull(comments);
                UserInterface.PrintCheeps(comments);
                return null;
            }
            catch
            {
                return "Could not find any observations.";
            }
        }

        public async static Task<string> StoreObservation(string observation, string location)
        {
            try
            {
                var res = await DBClient.PostAsJsonAsync("/observation", new ObservationRecord
                {
                    Author = Environment.UserName,
                    Observation = observation,
                    Location = location,
                    Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
                });
                res.EnsureSuccessStatusCode();
                return "Observation has been saved.";
            }
            catch
            {
                return "Failed to save observation.";
            }
        }

        public async static Task<string> TryComment(int observationId, string comment)
        {
            try
            {
                var res = await DBClient.PostAsJsonAsync($"/observation/{observationId}/comment", new CommentRecord()
                {
                    Author = Environment.UserName,
                    Comment = comment,
                    Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
                });
                res.EnsureSuccessStatusCode();
                return "Comment has been saved.";
            }
            catch (HttpRequestException e)
            {
                return e.StatusCode == System.Net.HttpStatusCode.BadRequest
                    ? string.Format("Observation id {0} does not exist", observationId)
                    : "Could not save comment.";
            }
        }

        public async static Task<string?> ReadComments(int observationId)
        {
            try
            {
                var comments = await DBClient.GetFromJsonAsync<List<CommentRecord>>("/observation/{0}/comments");
                var obs = await DBClient.GetFromJsonAsync<ObservationRecord>($"/observation/{observationId}");
                if (comments is not null && obs is not null)
                {
                    Console.WriteLine(obs);
                    Console.WriteLine();
                    UserInterface.PrintCheeps(comments);
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

        public async static Task<string> TryPropose(int observationId, string proposal)
        {
            var res = await DBClient.PostAsJsonAsync($"/observation/{observationId}/proposal", new ProposalRecord()
            {
                Author = Environment.UserName,
                TaxonID = proposal,
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            });
            return !res.IsSuccessStatusCode
                ? res.StatusCode != System.Net.HttpStatusCode.BadRequest ? "Could not save proposal." : await res.Content.ReadFromJsonAsync<string>() ?? "Unknown Error"
                : "Proposal has been saved.";
        }
    }
}
