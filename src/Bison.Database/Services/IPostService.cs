namespace Bison.Database.Services;

using System.Net.Mail;

using Bison.Models;

public interface IPostService
{
    public Task<PostDTO?> GetPostAsync(int id);

    public static PostDTO ToPostDTO(Post post)
    {
        if (post is Observation observation)
        {
            return new PostDTO(
                IObservationService.ToObservationDTO(observation),
                null,
                null
            );
        }
        else if (post is Comment comment)
        {
            return new PostDTO(
                null,
                ICommentService.ToCommentDTO(comment),
                null
            );
        }
        else if (post is Proposal proposal)
        {
            return new PostDTO(
                null,
                null,
                IProposalService.ToProposalDTO(proposal)
            );
        }

        throw new ArgumentException("Unknown post type", nameof(post));
    }

    public Task<Post?> ToPostAsync(PostDTO postDTO);
}

public record PostDTO(ObservationDTO? Observation, CommentDTO? Comment, ProposalDTO? Proposal)
{
    public int Id
    {
        get
        {
            if (Observation is ObservationDTO observation)
            {
                return observation.Id;
            }
            else if (Comment is CommentDTO comment)
            {
                return comment.Id;
            }
            else if (Proposal is ProposalDTO proposal)
            {
                return proposal.Id;
            }

            throw new ArgumentException("Unknown post type");
        }
    }
    public UserDTO Author
    {
        get
        {
            if (Observation is ObservationDTO observation)
            {
                return observation.Author;
            }
            else if (Comment is CommentDTO comment)
            {
                return comment.Author;
            }
            else if (Proposal is ProposalDTO proposal)
            {
                return proposal.Author;
            }

            throw new ArgumentException("Unknown post type");
        }
    }
    public string Text
    {
        get
        {
            if (Observation is ObservationDTO observation)
            {
                return observation.Description;
            }
            else if (Comment is CommentDTO comment)
            {
                return comment.Comment;
            }
            else if (Proposal is ProposalDTO proposal)
            {
                return proposal.Text;
            }

            throw new ArgumentException("Unknown post type");
        }
    }

    public string Type
    {
        get
        {
            if (Observation is not null)
            {
                return "Observation";
            }
            else if (Comment is not null)
            {
                return "Comment";
            }
            else if (Proposal is not null)
            {
                return "Proposal";
            }

            throw new ArgumentException("Unknown post type");
        }
    }
}
