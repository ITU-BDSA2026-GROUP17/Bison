namespace Bison.Database.Services;

using Bison.Models;

public interface ICommentService
{
    public Task<List<CommentDTO>> GetCommentsByObservationIdAsync(int observationId, int? limit = null, int? skip = null);
    public Task<CommentDTO> CreateCommentAsync(int authorId, int observationId, string comment);
}

public record CommentDTO(int Id, string CreatedAt, int ObservationId, string AuthorName, string Comment);
