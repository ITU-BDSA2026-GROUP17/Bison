namespace Bison.Models;

public record ObservationRecord
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
        return Author + " : " + Id + " @ " + GetAsDateTime().ToString() + ": " + Observation;
    }
}
