namespace Bison.Database.Services;

using Bison.Models;

public interface IProposalService
{
    public Task<List<Proposal>> GetProposalsByObservationIdAsync(int observationId, int? limit = null, int? skip = null);
    public Task<Proposal> CreateProposalAsync(int authorId, int observationId, string taxonId);
}
