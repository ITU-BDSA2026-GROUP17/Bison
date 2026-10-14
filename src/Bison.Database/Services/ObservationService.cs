using System.Globalization;

using Bison.Models;

namespace Bison.Database.Services;

public class ObservationService(IDatabaseRepository databaseRepository) : IObservationService
{
    private readonly IDatabaseRepository _repository = databaseRepository;

    //internal static readonly string DATE_FORMAT_STRING = "dd/MM/yy H:mm:ss";
    internal static readonly CultureInfo DATE_FORMAT = new("fr-FR");

    public async Task<ObservationDTO?> GetObservationByIdAsync(int id)
    {
        var obs = await _repository.GetObservation(id);
        return obs is null ? null :
            new ObservationDTO(
                obs.Id,
                obs.CreatedAt.ToString(DATE_FORMAT),
                obs.Author.Name,
                obs.Text
            );
    }
    public async Task<List<ObservationDTO>> GetObservationsByUserAsync(int userId, int? limit = null, int? skip = null)
    {
        return [.. (await _repository.GetObservationsByUser(userId, limit, skip)).Select(obs =>
            new ObservationDTO(
                obs.Id,
                obs.CreatedAt.ToString(DATE_FORMAT),
                obs.Author.Name,
                obs.Text
            )
        )];
    }
    public async Task<List<ObservationDTO>> GetObservationsAsync(int? limit = null, int? skip = null)
    {
        var obs = await _repository.GetObservations(limit, skip);
        return [.. obs.Select(obs =>
            new ObservationDTO(
                obs.Id,
                obs.CreatedAt.ToString(DATE_FORMAT),
                obs.Author.Name,
                obs.Text
            )
        )];
    }
    public async Task<ObservationDTO> CreateObservationAsync(int userId, string taxonId, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        var obs = await _repository.CreateObservation(userId, taxonId, description);
        return new ObservationDTO(
            obs.Id,
            obs.CreatedAt.ToString(DATE_FORMAT),
            obs.Author.Name,
            obs.Text
        );
    }
}
