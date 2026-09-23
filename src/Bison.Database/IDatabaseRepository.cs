using Bison.Models;

namespace Bison.Database;

public interface IDatabaseRepository
{
    public Task<User> CreateUser(string name);
    public Task<List<User>> GetAllUsers();

    public Task<Observation?> GetObservation(int id);
    public Task<List<Observation>> GetObservations(int? limit = null, int? skip = null);
    public Task<List<Observation>> GetObservationsByUser(int userId, int? limit = null, int? skip = null);
    public Task<Observation> CreateObservation(int userId, string observation, string location);

    public Task<List<Comment>> GetCommentsForObservation(int observationId, int? limit = null, int? skip = null);
    public Task<Comment> CreateComment(int authorId, int observationId, string comment);
    
    public Task<List<Proposal>> GetProposalsForObservation(int observationId, int? limit = null, int? skip = null);
    public Task<Proposal> CreateProposal(int authorId, int observationId, string taxonId);
}
