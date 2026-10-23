namespace Bison.Models;

public class Taxon
{
    public int Id { get; set; }
    public required string DWC_TaxonId { get; set; }
    public required string? VernacularName { get; set; }

    public required Taxon? Parent { get; set; }
    public List<Taxon> Children { get; set; } = [];
}
