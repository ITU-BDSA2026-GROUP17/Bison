namespace Bison.Database;

using System.Globalization;
using System.Reflection;

using Bison.Models;

using CsvHelper;

using Microsoft.Extensions.FileProviders;

public class Taxonomies
{
    internal readonly List<TaxonRecord> TaxonomiesList;

    public Taxonomies()
    {
        var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
        using var reader = embeddedProvider.GetFileInfo("resources/joined.csv").CreateReadStream();
        using var sr = new StreamReader(reader);
        using var csv = new CsvReader(sr, CultureInfo.InvariantCulture);
        TaxonomiesList = [.. csv.GetRecords<TaxonRecord>()];
    }

    public TaxonRecord? GetTaxonRecordByID(string id)
    {
        foreach (var taxon in TaxonomiesList)
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
        foreach (var taxon in TaxonomiesList)
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

    public IEnumerable<TaxonRecord> GetSubTaxons(TaxonRecord parent)
    {
        foreach (var taxon in TaxonomiesList)
        {
            if (taxon.ParentNameUsageID == parent.TaxonID)
            {
                yield return taxon;
            }
        }
    }
}
