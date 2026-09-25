namespace Bison.Database;

using Bison.Models;

public interface IDatabaseService
{
    public IEnumerable<Observation> ReadObservations(int? limit = null);
    public int StoreObservation(Observation record);

    public IEnumerable<Comment> ReadCommentsForObservation(int observationID, int? limit = null);
    public void StoreComment(Comment record);

    public IEnumerable<Proposal> ReadProposalsForObservation(int observationID, int? limit = null);
    public void StoreProposal(Proposal record);
}
