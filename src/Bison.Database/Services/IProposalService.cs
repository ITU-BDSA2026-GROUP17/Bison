using Bison.Models;

namespace Bison.Database.Services;

public interface IProposalService
{
    public Task<List<ProposalDTO>> GetProposalsByObservationIdAsync(int observationId, int? limit = null, int? skip = null);
    public Task<ProposalDTO> CreateProposalAsync(int authorId, int observationId, string taxonId, string text);

    public Task<ProposalDTO> ToProposalDTO(Proposal user);

    public Task<Proposal?> ToProposal(ProposalDTO userDTO);
}

public record ProposalDTO(int Id, string CreatedAt, int ObservationId, string AuthorName, string? TaxonName, string Text);
