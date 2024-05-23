using System.ComponentModel.DataAnnotations.Schema;

namespace Sup.Mm.Common.Entities;

public class Note
{
    [Column("id")]
    public long Id { get; set; }

    [Column("value")]
    public string Value { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}