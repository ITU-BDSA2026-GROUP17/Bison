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
    public async Task<ObservationDTO> CreateObservationAsync(string userId, string taxonId, string description, string? picturePath = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        var obs = await _repository.CreateObservation(userId, taxonId, description, picturePath);
        return IObservationService.ToObservationDTO(obs);
    }
    public async Task<ObservationDTO?> EditObservationAsync(int obsId, string? newTaxonId = null, string? newDescription = null, string? newPicturePath = null)
    {
        await _repository.EditObservation(obsId, newTaxonId, newDescription, newPicturePath);
        return await GetObservationByIdAsync(obsId);
    }

    public async Task<List<SubPostDTO>> GetSubPostsAsync(int id, int? limit = null, int? skip = null)
    {
        var obs = await _repository.GetSubpostsForObservation(id, limit, skip);
        return [.. obs.Select(IObservationService.ToPostDTO)];
    }

    public async Task<Observation?> ToObservationAsync(ObservationDTO observationDTO)
    {
        return await _repository.GetObservation(observationDTO.Id);
    }
    public async Task<Post?> ToPostAsync(SubPostDTO subPostDTO)
    {
        if (subPostDTO.CommentDTO is not null == subPostDTO.ProposalDTO is not null)
        {
            throw new ArgumentException("SubPostDTO contains either both or none of comment or proposal DTOs");
        }
        if (subPostDTO.CommentDTO is CommentDTO commentDTO)
        {
            return await new CommentService(_repository).ToCommentAsync(commentDTO);
        }
        else if (subPostDTO.ProposalDTO is ProposalDTO proposalDTO)
        {
            return await new ProposalService(_repository).ToProposalAsync(proposalDTO);
        }
        throw new ArgumentException("SubPostDTO contains neither comment nor proposal DTOs");
    }
}
