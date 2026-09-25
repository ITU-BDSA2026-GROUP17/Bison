namespace Bison.Database;

public class ObservationDoesNotExistException(int observationId) : Exception($"Observation with ID {observationId} does not exist")
{
    public readonly int ObservationId = observationId;
}