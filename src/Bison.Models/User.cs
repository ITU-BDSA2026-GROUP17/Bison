namespace Bison.Models;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
public sealed class User : IEquatable<User>
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public required List<Comment> Comments { get; set; }
    public required List<Observation> Observations { get; set; }
    public required List<Proposal> Proposals { get; set; }

    public bool Equals(User? other)
    {
        return other is null ?
            this is null :
            this is not null && Id == other.Id && Name == other.Name;
    }

    public override bool Equals(object? obj)
    {
        return obj is User other && Equals(other);
    }
}
