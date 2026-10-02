using Bison.Database.Exceptions;
using Bison.Models;

namespace Bison.Database.Services;

public class ProposalService(IDatabaseRepository databaseRepository) : IProposalService
{
    private readonly static Taxonomies Taxonomies = new();

    private readonly IDatabaseRepository _repository = databaseRepository;

    public Task<List<Proposal>> GetProposalsByObservationIdAsync(int observationId, int? limit = null, int? skip = null)
    {
        return _repository.GetProposalsForObservation(observationId, limit, skip);
    }
    public Task<Proposal> CreateProposalAsync(int authorId, int observationId, string taxonId)
    {
        return Taxonomies.GetTaxonRecordByID(taxonId) is not null
            ? _repository.CreateProposal(authorId, observationId, taxonId)
            : throw new TaxonIdDoesNotExistException(taxonId);
    }
}
