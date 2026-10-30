using Bison.Models;

namespace Bison.Database.Services;

public interface IObservationService
{
    public Task<ObservationDTO?> GetObservationByIdAsync(int id);
    public Task<List<ObservationDTO>> GetObservationsByUserAsync(int userId, int? limit = null, int? skip = null);
    public Task<List<ObservationDTO>> GetObservationsByTaxonAsync(string taxonId, int? limit = null, int? skip = null);
    public Task<List<ObservationDTO>> GetObservationsAsync(int? limit = null, int? skip = null);
    public Task<ObservationDTO> CreateObservationAsync(int userId, string taxonId, string description);

    public static ObservationDTO ToObservationDTO(Observation observation)
    {
        return new ObservationDTO(
            observation.Id,
            observation.CreatedAt.ToString(IDatabaseRepository.DATE_FORMAT),
            IUserService.ToUserDTO(observation.Author),
            observation.Taxon.VernacularName,
            observation.Text
        );
    }
    public Task<Observation?> ToObservationAsync(ObservationDTO observationDTO);
}

// CreatedAt should preferably be a DateTime if localization across different date formats is needed
// However, for now it will be ISO 8601 string
public record ObservationDTO(int Id, string CreatedAt, UserDTO Author, string? TaxonName, string Description);
