using Bison.Models;

namespace Bison.Database.Services;

public class ObservationService(IDatabaseRepository databaseRepository) : IObservationService
{
    private readonly IDatabaseRepository _repository = databaseRepository;

    public Task<Observation?> GetObservationByIdAsync(int id)
    {
        return _repository.GetObservation(id);
    }
    public Task<List<Observation>> GetObservationsByUserAsync(int userId, int? limit = null, int? skip = null)
    {
        return _repository.GetObservationsByUser(userId, limit, skip);
    }
    public Task<List<Observation>> GetObservationsAsync(int? limit = null, int? skip = null)
    {
        return _repository.GetObservations(limit, skip);
    }
    public Task<Observation> CreateObservationAsync(int userId, string description, string location)
    {
        return _repository.CreateObservation(userId, description, location);
    }
}
