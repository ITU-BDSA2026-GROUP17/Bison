namespace Bison.Models;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
public sealed class Taxon : IEquatable<Taxon>
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
{
    public int Id { get; set; }
    public required string DWC_TaxonId { get; set; }
    public required string? VernacularName { get; set; }

    public required Taxon? Parent { get; set; }
    public List<Taxon> Children { get; set; } = [];

    public bool Equals(Taxon? other)
    {
        return other is null ?
            this is null :
            this is not null && Id == other.Id &&
                DWC_TaxonId == other.DWC_TaxonId &&
                VernacularName == other.VernacularName;
    }

    public override bool Equals(object? obj)
    {
        return obj is Taxon other && Equals(other);
    }

    public bool IsSubTaxon(Taxon ancestor)
    {
        return
            Id == ancestor.Id ||
            Parent is not null && Parent.IsSubTaxon(ancestor);
    }
}
