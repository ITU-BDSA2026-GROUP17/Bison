namespace Bison.Database;

using Bison.Models;

using CsvHelper;


using Microsoft.Extensions.FileProviders;

using System.Globalization;

using System.Reflection;

public class Taxonomies
{
    private readonly List<TaxonRecord> _taxonomies;

    public Taxonomies() {
        var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
        using var reader = embeddedProvider.GetFileInfo("./resources/joined.csv").CreateReadStream();
        using var sr = new StreamReader(reader);
        using var csv = new CsvReader(sr, CultureInfo.InvariantCulture);
        _taxonomies = [.. csv.GetRecords<TaxonRecord>()];
    }

    #nullable enable
    public TaxonRecord? GetTaxonRecordByID(string id)
    {
        foreach(var taxon in _taxonomies)
        {
            if (taxon.TaxonID == id)
            {
                return taxon;
            }
        }
        return null;
    }

    public TaxonRecord? GetTaxonRecordByName(string name)
    {
        foreach(var taxon in _taxonomies)
        {
            if (taxon.VernacularName == name)
            {
                return taxon;
            }
        }
        return null;
    }

    public TaxonRecord? GetSuperTaxon(TaxonRecord child)
    {
        return GetTaxonRecordByID(child.ParentNameUsageID);
    }
    #nullable restore
    
    public IEnumerable<TaxonRecord> GetSubTaxons(TaxonRecord parent)
    {
        foreach (var taxon in _taxonomies)
        {
            if (taxon.ParentNameUsageID == parent.TaxonID)
            {
                yield return taxon;
            }
        }
    }
}