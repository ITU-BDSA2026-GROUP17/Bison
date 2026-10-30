using Bison.Models;

namespace Bison.Database.Services;

public class CommentService(IDatabaseRepository databaseRepository) : ICommentService
{
    private readonly IDatabaseRepository _repository = databaseRepository;

    public async Task<List<CommentDTO>> GetCommentsByObservationIdAsync(int observationId, int? limit = null, int? skip = null)
    {
        return [..
            (await _repository.GetCommentsForObservation(observationId, limit, skip))
            .Select(ICommentService.ToCommentDTO)
        ];
    }
    public async Task<CommentDTO> CreateCommentAsync(string authorId, int observationId, string comment)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(comment);
        var cmt = await _repository.CreateComment(authorId, observationId, comment);
        return ICommentService.ToCommentDTO(cmt);
    }
    public async Task<Comment?> ToCommentAsync(CommentDTO commentDTO)
    {
        var comments = await _repository.GetCommentsForObservation(commentDTO.ObservationId);
        foreach (var comment in comments)
        {
            if (comment.Id == commentDTO.Id)
            {
                return comment;
            }
        }
        return null;
    }
}
