namespace Bison.Models;

public sealed record CommentRecord
{
    public required int ObservationId { get; set; }
    public required string Author { get; set; }
    public required string Comment { get; set; }
    public int Timestamp { get; set; }

    public DateTime GetAsDateTime()
    {
        return Utilities.UnixTimeStampToDateTime(Timestamp);
    }

    public override string ToString()
    {
        return string.Format("{0} @ {1}: {2}", Author, GetAsDateTime().ToString(), Comment);
    }
}
