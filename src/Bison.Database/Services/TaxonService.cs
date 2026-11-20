
using Bison.Models;

namespace Bison.Database.Services;

public class TaxonService(IDatabaseRepository databaseRepository) : ITaxonService
{
    private readonly IDatabaseRepository _repository = databaseRepository;

    public async Task<TaxonDTO?> GetTaxonById(string taxonId)
    {
        var taxon = await _repository.GetTaxonByDWCId(taxonId);
        return taxon is not null ? ITaxonService.ToTaxonDTO(taxon) : null;
    }

    public async Task<TaxonDTO?> GetTaxonByVernacularName(string vernacularName)
    {
        var taxon = await _repository.GetTaxonByVernacularName(vernacularName);
        return taxon is not null ? ITaxonService.ToTaxonDTO(taxon) : null;
    }

    public async Task<List<TaxonDTO>> GetTaxonsAsync(int? limit = null, int? skip = null)
    {
        return [..
            (await _repository.GetTaxons(limit, skip))
            .Select(ITaxonService.ToTaxonDTO)
        ];
    }

    public Task<Taxon?> ToTaxonAsync(TaxonDTO taxonDto)
    {
        return _repository.GetTaxonByDWCId(taxonDto.DwCId);
    }
}
