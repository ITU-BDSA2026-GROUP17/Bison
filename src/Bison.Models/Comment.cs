namespace Bison.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public sealed class Comment
{
    public int Id { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    public required User Author { get; set; }
    public Observation Observation { get; set; }
    [MaxLength(512)]
    public required string Text { get; set; }
}
