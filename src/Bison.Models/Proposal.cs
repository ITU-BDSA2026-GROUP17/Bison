namespace Bison.Models;

using System.ComponentModel.DataAnnotations;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
public sealed class Proposal : IEquatable<Proposal>
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }

    public required User Author { get; set; }
    public required Observation Observation { get; set; }
    [MaxLength(128)]
    public required string TaxonId { get; set; }

    public bool Equals(Proposal? other)
    {
        return other is null ?
            this is null :
            this is not null && Id == other.Id &&
                Author.Equals(other.Author) && CreatedAt.Equals(other.CreatedAt) &&
                Observation.Equals(other.Observation) && TaxonId == other.TaxonId;
    }

    public override bool Equals(object? obj)
    {
        return obj is Proposal other && Equals(other);
    }
}
