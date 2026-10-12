using Bison.Database.Exceptions;
using Bison.Models;

namespace Bison.Database.Services;

public class ProposalService(IDatabaseRepository databaseRepository) : IProposalService
{
    private readonly IDatabaseRepository _repository = databaseRepository;

    public Task<List<Proposal>> GetProposalsByObservationIdAsync(int observationId, int? limit = null, int? skip = null)
    {
        return _repository.GetProposalsForObservation(observationId, limit, skip);
    }
    public Task<Proposal> CreateProposalAsync(int authorId, int observationId, string taxonId, string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        return _repository.CreateProposal(authorId, observationId, taxonId, text);
    }
}
