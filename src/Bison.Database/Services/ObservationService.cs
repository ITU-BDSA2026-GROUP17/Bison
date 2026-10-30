using Bison.Database.Exceptions;
using Bison.Models;
using Bison.Verified.Services;

namespace Bison.Database.Services;

public class ObservationService(IDatabaseRepository databaseRepository) : IObservationService
{
    private readonly IDatabaseRepository _repository = databaseRepository;

    public async Task<ObservationDTO?> GetObservationByIdAsync(int id)
    {
        var obs = await _repository.GetObservation(id);
        return obs is null ? null : IObservationService.ToObservationDTO(obs);
    }
    public async Task<List<ObservationDTO>> GetObservationsByUserAsync(string userId, int? limit = null, int? skip = null)
    {
        return [.. (await _repository.GetObservationsByUser(userId, limit, skip))
            .Select(IObservationService.ToObservationDTO)
        ];
    }
    public async Task<List<ObservationDTO>> GetObservationsByTaxonAsync(string taxonId, int? limit = null, int? skip = null)
    {
        await _repository.GetTaxons(); // load all taxons in memory (else ef core might show bad ones)
        var taxon = await _repository.GetTaxonByDWCId(taxonId) ?? throw new TaxonIdDoesNotExistException(taxonId);
        var filtered = FilterByService.FilterByTaxon(
            taxon,
            await _repository.GetObservations(limit, skip)
        );
        return [.. filtered.Select(IObservationService.ToObservationDTO)];
    }
    public async Task<List<ObservationDTO>> GetObservationsAsync(int? limit = null, int? skip = null)
    {
        var obs = await _repository.GetObservations(limit, skip);
        return [.. obs.Select(IObservationService.ToObservationDTO)];
    }
    public async Task<ObservationDTO> CreateObservationAsync(string userId, string taxonId, string description)
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
