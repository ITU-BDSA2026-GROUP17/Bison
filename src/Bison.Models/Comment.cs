namespace Bison.Models;

using System.ComponentModel.DataAnnotations;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
public sealed class Comment : IEquatable<Comment>
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }

    public required User Author { get; set; }
    public required Observation Observation { get; set; }
    [MaxLength(512)]
    public required string Text { get; set; }

    public bool Equals(Comment? other)
    {
        return other is null ?
            this is null :
            this is not null && Id == other.Id &&
                Author.Equals(other.Author) && CreatedAt.Equals(other.CreatedAt) &&
                Observation.Equals(other.Observation) && Text == other.Text;
    }

    public override bool Equals(object? obj)
    {
        return obj is Comment other && Equals(other);
    }
}
