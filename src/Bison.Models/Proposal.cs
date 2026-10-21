namespace Bison.Models;

using System.ComponentModel.DataAnnotations;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
public sealed class Proposal : Post, IEquatable<Proposal>
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
{
    public required Observation Observation { get; set; }
    public required Taxon Taxon { get; set; }

    public bool Equals(Proposal? other)
    {
        return base.Equals(other) &&
                Observation.Equals(other.Observation) && Taxon.Equals(other.Taxon);
    }

    public override bool Equals(object? obj)
    {
        return obj is Proposal other && Equals(other);
    }
}
