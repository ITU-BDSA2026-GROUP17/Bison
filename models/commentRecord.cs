namespace Bison.Models;

public record CommentRecord
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
        return Author + " @ " + GetAsDateTime().ToString() + ": " + Comment;
    }
}
