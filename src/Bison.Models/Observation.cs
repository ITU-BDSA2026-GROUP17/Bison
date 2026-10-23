namespace Bison.Models;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
public sealed class Observation : Post, IEquatable<Observation>
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
{
    public required Taxon Taxon { get; set; }

    public List<Comment> Comments { get; set; } = [];
    public List<Proposal> Proposals { get; set; } = [];

    public bool Equals(Observation? other)
    {
        return base.Equals(other);
    }

    public override bool Equals(object? obj)
    {
        return obj is Observation other && Equals(other);
    }

    public Taxon GetTaxon()
    {
        return Taxon;
    }
}
