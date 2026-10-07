namespace Bison.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public sealed class Observation
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
}
