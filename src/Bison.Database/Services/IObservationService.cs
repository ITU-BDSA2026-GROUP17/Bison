namespace Bison.Database.Services;

public interface IObservationService
{
    public Task<ObservationDTO?> GetObservationByIdAsync(int id);
    public Task<List<ObservationDTO>> GetObservationsByUserAsync(int userId, int? limit = null, int? skip = null);
    public Task<List<ObservationDTO>> GetObservationsAsync(int? limit = null, int? skip = null);
    public Task<ObservationDTO> CreateObservationAsync(int userId, string taxonId, string description);
}

// CreatedAt should preferably be a DateTime if localization across different date formats is needed
// However, for now it will be ISO 8601 string
public record ObservationDTO(int Id, string CreatedAt, string AuthorName, string Description);