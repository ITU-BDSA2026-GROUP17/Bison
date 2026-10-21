using Bison.Models;

namespace Bison.Database.Services;

public class ObservationService(IDatabaseRepository databaseRepository) : IObservationService
{
    private readonly IDatabaseRepository _repository = databaseRepository;

    public async Task<ObservationDTO?> GetObservationByIdAsync(int id)
    {
        var obs = await _repository.GetObservation(id);
        return obs is null ? null : IObservationService.ToObservationDTO(obs);
    }
    public async Task<List<ObservationDTO>> GetObservationsByUserAsync(int userId, int? limit = null, int? skip = null)
    {
        return [.. (await _repository.GetObservationsByUser(userId, limit, skip))
            .Select(IObservationService.ToObservationDTO)
        ];
    }
    public async Task<List<ObservationDTO>> GetObservationsAsync(int? limit = null, int? skip = null)
    {
        var obs = await _repository.GetObservations(limit, skip);
        return [.. obs.Select(IObservationService.ToObservationDTO)];
    }
    public async Task<ObservationDTO> CreateObservationAsync(int userId, string taxonId, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        var obs = await _repository.CreateObservation(userId, taxonId, description);
        return IObservationService.ToObservationDTO(obs);
    }
    public async Task<Observation?> ToObservationAsync(ObservationDTO observationDTO)
    {
        return await _repository.GetObservation(observationDTO.Id);
    }
}
