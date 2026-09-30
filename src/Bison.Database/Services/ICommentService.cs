namespace Bison.Database.Services;

using Bison.Models;

public interface ICommentService
{
    public Task<List<Comment>> GetCommentsByObservationIdAsync(int observationId, int? limit = null, int? skip = null);
    public Task<Comment> CreateCommentAsync(int authorId, int observationId, string comment);
}
