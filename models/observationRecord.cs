namespace Bison.Models;

public sealed record ObservationRecord
{
    public required int Id { get; set; }
    public required string Author { get; set; }
    public required string Observation { get; set; }
    public int Timestamp { get; set; }

    public DateTime GetAsDateTime()
    {
        return Utilities.UnixTimeStampToDateTime(Timestamp);
    }

    public override string ToString()
    {
        return string.Format("{0} : {1} @ {2}: {3}", Author, Id, GetAsDateTime().ToString(), Observation);
    }
}
