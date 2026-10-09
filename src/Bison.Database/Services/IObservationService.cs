namespace Bison.Database.Services;

using Bison.Models;

public interface IObservationService
{
    public Task<Observation?> GetObservationByIdAsync(int id);
    public Task<List<Observation>> GetObservationsByUserAsync(int userId, int? limit = null, int? skip = null);
    public Task<List<Observation>> GetObservationsAsync(int? limit = null, int? skip = null);
    public Task<Observation> CreateObservationAsync(int userId, string description, string location);
}
