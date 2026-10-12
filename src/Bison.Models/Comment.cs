namespace Bison.Models;

using System.ComponentModel.DataAnnotations;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
public sealed class Comment : Post, IEquatable<Comment>
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
{
    public required Observation Observation { get; set; }

    public bool Equals(Comment? other)
    {
        return base.Equals(other) && Observation.Equals(other.Observation);
    }

    public override bool Equals(object? obj)
    {
        return obj is Comment other && Equals(other);
    }
}
