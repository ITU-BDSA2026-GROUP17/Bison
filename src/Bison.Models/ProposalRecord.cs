namespace Bison.Models;

using Bison.Utilities;

public sealed record ProposalRecord
{
    public int ObservationId { get; set; }
    public required string Author { get; set; }
    public required string TaxonID { get; set; }
    public required int Timestamp { get; set; }

    public DateTime GetAsDateTime()
    {
        return DateTimeUtilities.UnixTimeStampToDateTime(Timestamp);
    }

    public override string ToString()
    {
        return string.Format("{0} @ {1}: {2}", Author, GetAsDateTime().ToString(), TaxonID);
    }
}
