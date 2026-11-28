using Bison.Models;

namespace Bison.Database.Services;

public interface ITaxonService
{
    public Task<TaxonDTO?> GetTaxonById(string taxonId);
    public Task<TaxonDTO?> GetTaxonByVernacularName(string vernacularName);
    public Task<List<TaxonDTO>> GetTaxonsAsync(int? limit = null, int? skip = null);

    public static TaxonDTO ToTaxonDTO(Taxon taxon)
    {
        return new TaxonDTO(
            taxon.DWC_TaxonId,
            taxon.VernacularName,
            taxon.Parent?.DWC_TaxonId
        );
    }
    public Task<Taxon?> ToTaxonAsync(TaxonDTO taxonDto);
}

// CreatedAt should preferably be a DateTime if localization across different date formats is needed
// However, for now it will be ISO 8601 string
public record TaxonDTO(string DwCId, string? VernacularName, string? ParentId);
