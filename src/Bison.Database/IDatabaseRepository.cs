using System.Globalization;

using Bison.Models;

namespace Bison.Database;

public interface IDatabaseRepository
{
    //internal static readonly string DATE_FORMAT_STRING = "dd/MM/yy H:mm:ss";
    internal static readonly CultureInfo DATE_FORMAT = new("fr-FR");

    public Task<User> CreateUser(string name, string email);
    public Task<User?> GetUserById(int userId);
    public Task<User?> GetUserByName(string name);

    public Task<Observation?> GetObservation(int id);
    public Task<List<Observation>> GetObservations(int? limit = null, int? skip = null);
    public Task<List<Observation>> GetObservationsByUser(int userId, int? limit = null, int? skip = null);
    public Task<Observation> CreateObservation(int userId, string taxonId, string observation);

    public Task<List<Comment>> GetCommentsForObservation(int observationId, int? limit = null, int? skip = null);
    public Task<Comment> CreateComment(int authorId, int observationId, string comment);

    public Task<List<Proposal>> GetProposalsForObservation(int observationId, int? limit = null, int? skip = null);
    public Task<Proposal> CreateProposal(int authorId, int observationId, string taxonId, string text);
}
