using Bison.Models;

namespace Bison.Database.Services;

public class CommentService(IDatabaseRepository databaseRepository) : ICommentService
{
    private readonly IDatabaseRepository _repository = databaseRepository;

    public Task<List<Comment>> GetCommentsByObservationIdAsync(int observationId, int? limit = null, int? skip = null)
    {
        return _repository.GetCommentsForObservation(observationId, limit, skip);
    }
    public Task<Comment> CreateCommentAsync(int authorId, int observationId, string comment)
    {
        return _repository.CreateComment(authorId, observationId, comment);
    }
}
