namespace Bison.Models;

using CsvHelper.Configuration.Attributes;

public record TaxonRecord
{
    [Name("dwc:taxonID")]
    public required string TaxonID { get; init; }

    [Name("dwc:vernacularName")]
    public required string VernacularName { get; init; }

    [Name("dwc:parentNameUsageID")]
    public required string ParentNameUsageID { get; init; }
}
