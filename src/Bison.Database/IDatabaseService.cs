namespace Bison.Database;

using Bison.Models;

public interface IDatabaseService
{
    public IEnumerable<ObservationRecord> ReadObservations(int? limit = null);
    public int StoreObservation(ObservationRecord record);

    public IEnumerable<CommentRecord> ReadCommentsForObservation(int observationID, int? limit = null);
    public void StoreComment(CommentRecord record);
}
