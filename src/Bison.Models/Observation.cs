namespace Bison.Models;

using System.ComponentModel.DataAnnotations;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
public sealed class Observation : IEquatable<Observation>
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }

    public required User Author { get; set; }
    [MaxLength(512)]
    public required string Text { get; set; }
    [MaxLength(512)]
    public required string Location { get; set; }

    public required List<Comment> Comments { get; set; }
    public required List<Proposal> Proposals { get; set; }

    public bool Equals(Observation? other)
    {
        return other is null ?
            this is null :
            this is not null && Id == other.Id &&
                Author.Equals(other.Author) && CreatedAt.Equals(other.CreatedAt) &&
                Text == other.Text && Location == other.Location;
    }

    public override bool Equals(object? obj)
    {
        return obj is Observation other && Equals(other);
    }
}
