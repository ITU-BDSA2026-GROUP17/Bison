namespace Bison.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public sealed class Proposal
{
    public int Id { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required DateTime CreatedAt { get; set; }

    public required User Author { get; set; }
    public Observation Observation { get; set; }
    [MaxLength(128)]
    public required string TaxonId { get; set; }
}
