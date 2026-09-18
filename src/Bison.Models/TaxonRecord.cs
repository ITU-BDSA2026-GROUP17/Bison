namespace Bison.Models;

using CsvHelper.Configuration.Attributes;

public record TaxonRecord
{
    [Name("dwc:taxonID")]
    public string TaxonID { get; init; }

    [Name("dwc:vernacularName")]
    public string VernacularName { get; init; }

    [Name("dwc:parentNameUsageID")]
    public string ParentNameUsageID { get; init; }
}