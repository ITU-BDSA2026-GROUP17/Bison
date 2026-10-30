using System.Globalization;

using Bison.Models;

namespace Bison.Database;

public interface IDatabaseRepository
{
    //internal static readonly string DATE_FORMAT_STRING = "dd/MM/yyyy HH:mm:ss";
    internal static readonly CultureInfo DATE_FORMAT = new("fr-FR");

    public Task<Taxon?> GetTaxonByDWCId(string taxonId);
    public Task<Taxon?> GetTaxonByNumericId(int taxonId);
    public Task<List<Taxon>> GetTaxons(int? limit = null, int? skip = null);

    public Task<User> CreateUser(string username, string email, string name);
    public Task<User?> GetUserById(string userId);
    public Task<User?> GetUserByUserName(string name);

    public Task<Observation?> GetObservation(int id);
    public Task<List<Observation>> GetObservations(int? limit = null, int? skip = null);
    public Task<List<Observation>> GetObservationsByUser(string userId, int? limit = null, int? skip = null);
    public Task<Observation> CreateObservation(string userId, string taxonId, string observation);

    public Task<List<Comment>> GetCommentsForObservation(int observationId, int? limit = null, int? skip = null);
    public Task<Comment> CreateComment(string authorId, int observationId, string comment);

    public Task<List<Proposal>> GetProposalsForObservation(int observationId, int? limit = null, int? skip = null);
    public Task<Proposal> CreateProposal(string authorId, int observationId, string taxonId, string text);
}
