namespace Bison.Verified.Services;

using Bison.Models;

public class FilterByService
{
    public static IList<Observation> FilterByTaxon(Taxon root, IList<Observation> observations)
    {
        return [.. __default.FilterBy(root, new Dafny.ArraySequence<Observation>(observations.ToArray()))];
    }
}
