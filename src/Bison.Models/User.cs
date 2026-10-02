namespace Bison.Models;

public sealed class User
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public required List<Comment> Comments { get; set; }
    public required List<Observation> Observations { get; set; }
    public required List<Proposal> Proposals { get; set; }
}
