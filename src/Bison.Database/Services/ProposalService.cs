using Bison.Database.Exceptions;
using Bison.Models;

namespace Bison.Database.Services;

public class ProposalService(IDatabaseRepository databaseRepository) : IProposalService
{
    private readonly IDatabaseRepository _repository = databaseRepository;

    public async Task<List<ProposalDTO>> GetProposalsByObservationIdAsync(int observationId, int? limit = null, int? skip = null)
    {
        return [..
            (await _repository.GetProposalsForObservation(observationId, limit, skip))
            .Select(ppl => new ProposalDTO(
                ppl.Id,
                ppl.CreatedAt.ToString(ObservationService.DATE_FORMAT),
                ppl.Observation.Id,
                ppl.Author.Name,
                ppl.Taxon.VernacularName,
                ppl.Text
            ))
        ];
    }
    public async Task<ProposalDTO> CreateProposalAsync(int authorId, int observationId, string taxonId, string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        var ppl = await _repository.CreateProposal(authorId, observationId, taxonId, text);
        return new ProposalDTO(
            ppl.Id,
            ppl.CreatedAt.ToString(ObservationService.DATE_FORMAT),
            ppl.Observation.Id,
            ppl.Author.Name,
            ppl.Taxon.VernacularName,
            ppl.Text
        );
    }
}
