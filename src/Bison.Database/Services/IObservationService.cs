using Bison.Models;

namespace Bison.Database.Services;

public interface IObservationService
{
    public Task<ObservationDTO?> GetObservationByIdAsync(int id);
    public Task<List<ObservationDTO>> GetObservationsByUserAsync(string userId, int? limit = null, int? skip = null);
    public Task<List<ObservationDTO>> GetObservationsByTaxonAsync(string taxonId, int? limit = null, int? skip = null);
    public Task<List<ObservationDTO>> GetObservationsAsync(int? limit = null, int? skip = null);
    public Task<ObservationDTO> CreateObservationAsync(string userId, string taxonId, string description, string? picturePath = null);

    public Task<List<SubPostDTO>> GetSubPostsAsync(int id, int? limit = null, int? skip = null);

    public static ObservationDTO ToObservationDTO(Observation observation)
    {
        return new ObservationDTO(
            observation.Id,
            observation.CreatedAt.ToString(IDatabaseRepository.DATE_FORMAT),
            IUserService.ToUserDTO(observation.Author),
            ITaxonService.ToTaxonDTO(observation.Taxon),
            observation.Text,
            observation.PicturePath
        );
    }
    public static SubPostDTO ToPostDTO(Post post)
    {
        if (post is Comment comment)
        {
            return new SubPostDTO(
                comment.Observation.Id,
                ICommentService.ToCommentDTO(comment),
                null
            );
        }
        else if (post is Proposal proposal)
        {
            return new SubPostDTO(
                proposal.Observation.Id,
                null,
                IProposalService.ToProposalDTO(proposal)
            );
        }
        throw new ArgumentException("Invalid subpost given", nameof(post));
    }
    public Task<Observation?> ToObservationAsync(ObservationDTO observationDTO);
    public Task<Post?> ToPostAsync(SubPostDTO subPostDTO);
}

// CreatedAt should preferably be a DateTime if localization across different date formats is needed
// However, for now it will be ISO 8601 string
public record ObservationDTO(int Id, string CreatedAt, UserDTO Author, TaxonDTO Taxon, string Description, string? PicturePath = null);

public record SubPostDTO(int ObservationId, CommentDTO? CommentDTO, ProposalDTO? ProposalDTO);
