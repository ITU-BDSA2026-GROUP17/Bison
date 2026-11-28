using Bison.Models;

namespace Bison.Database.Services;

public class PostService(IDatabaseRepository databaseRepository) : IPostService
{
    private readonly IDatabaseRepository _repository = databaseRepository;

    public async Task<PostDTO?> GetPostAsync(int id)
    {
        var obs = await _repository.GetPost(id);
        return obs is null ? null : IPostService.ToPostDTO(obs);
    }

    public async Task<Post?> ToPostAsync(PostDTO postDTO)
    {
        var count = new bool[] { postDTO.Comment is not null, postDTO.Proposal is not null, postDTO.Observation is not null }.Count(c => c);
        if (count != 1)
        {
            throw new ArgumentException("PostDTO contains either multiple or none of comment, proposal, or observation DTOs");
        }
        if (postDTO.Observation is ObservationDTO observationDTO)
        {
            return await new ObservationService(_repository).ToObservationAsync(observationDTO);
        }
        else if (postDTO.Comment is CommentDTO commentDTO)
        {
            return await new CommentService(_repository).ToCommentAsync(commentDTO);
        }
        else if (postDTO.Proposal is ProposalDTO proposalDTO)
        {
            return await new ProposalService(_repository).ToProposalAsync(proposalDTO);
        }
        throw new ArgumentException("PostDTO contains no DTOs");
    }
}
