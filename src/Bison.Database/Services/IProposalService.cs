using Bison.Models;

namespace Bison.Database.Services;

public interface IProposalService
{
    public Task<List<ProposalDTO>> GetProposalsByObservationIdAsync(int observationId, int? limit = null, int? skip = null);
    public Task<ProposalDTO> CreateProposalAsync(int authorId, int observationId, string taxonId, string text);

    public static ProposalDTO ToProposalDTO(Proposal proposal)
    {
        return new ProposalDTO(
            proposal.Id,
            proposal.CreatedAt.ToString(IDatabaseRepository.DATE_FORMAT),
            proposal.Observation.Id,
            IUserService.ToUserDTO(proposal.Author),
            proposal.Taxon.VernacularName,
            proposal.Text
        );
    }

    public Task<Proposal?> ToProposalAsync(ProposalDTO proposalDTO);
}

public record ProposalDTO(int Id, string CreatedAt, int ObservationId, UserDTO Author, string? TaxonName, string Text);
