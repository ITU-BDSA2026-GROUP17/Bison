namespace Bison.Database.Services;

public class CommentService(IDatabaseRepository databaseRepository) : ICommentService
{
    private readonly IDatabaseRepository _repository = databaseRepository;

    public async Task<List<CommentDTO>> GetCommentsByObservationIdAsync(int observationId, int? limit = null, int? skip = null)
    {
        return [..
            (await _repository.GetCommentsForObservation(observationId, limit, skip))
            .Select(cmt => new CommentDTO(
                cmt.Id,
                cmt.CreatedAt.ToString(ObservationService.DATE_FORMAT_STRING),
                cmt.Observation.Id,
                cmt.Author.Name,
                cmt.Text
            ))
        ];
    }
    public async Task<CommentDTO> CreateCommentAsync(int authorId, int observationId, string comment)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(comment);
        var cmt = await _repository.CreateComment(authorId, observationId, comment);
        return new CommentDTO(
            cmt.Id,
            cmt.CreatedAt.ToString(ObservationService.DATE_FORMAT_STRING),
            cmt.Observation.Id,
            cmt.Author.Name,
            cmt.Text
        );
    }
}
