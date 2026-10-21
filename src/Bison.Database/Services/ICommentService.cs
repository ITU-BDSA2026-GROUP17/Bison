namespace Bison.Database.Services;

using Bison.Models;

public interface ICommentService
{
    public Task<List<CommentDTO>> GetCommentsByObservationIdAsync(int observationId, int? limit = null, int? skip = null);
    public Task<CommentDTO> CreateCommentAsync(int authorId, int observationId, string comment);

    public static CommentDTO ToCommentDTO(Comment comment)
    {
        return new CommentDTO(
            comment.Id,
            comment.CreatedAt.ToString(IDatabaseRepository.DATE_FORMAT),
            comment.Observation.Id,
            IUserService.ToUserDTO(comment.Author),
            comment.Text
        );
    }

    public Task<Comment?> ToCommentAsync(CommentDTO commentDTO);
}

public record CommentDTO(int Id, string CreatedAt, int ObservationId, UserDTO Author, string Comment);
