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
            .Select(IProposalService.ToProposalDTO)
        ];
    }
    public async Task<ProposalDTO> CreateProposalAsync(int authorId, int observationId, string taxonId, string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        var ppl = await _repository.CreateProposal(authorId, observationId, taxonId, text);
        return IProposalService.ToProposalDTO(ppl);
    }

    public async Task<Proposal?> ToProposalAsync(ProposalDTO proposalDTO)
    {
        var proposals = await _repository.GetProposalsForObservation(proposalDTO.ObservationId);
        foreach (var proposal in proposals)
        {
            if (proposal.Id == proposalDTO.Id)
            {
                return proposal;
            }
        }
        return null;
    }
}
