namespace Bison.Models;

using System.ComponentModel.DataAnnotations;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
public class Post : IEquatable<Post>
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
{
    public int Id { get; set; }
    public required string CreatedAt { get; set; } //should use ObservationService.DATE_FORMAT for the format of it

    public required User Author { get; set; }
    [MaxLength(512)]
    public required string Text { get; set; } 
    public bool Equals(Post? other)
    {
        return other is null ?
            this is null :
            this is not null && Id == other.Id &&
                Author.Equals(other.Author) && CreatedAt.Equals(other.CreatedAt) &&
                Text == other.Text;
    }

    public override bool Equals(object? obj)
    {
        return obj is Post other && Equals(other);
    }
}
