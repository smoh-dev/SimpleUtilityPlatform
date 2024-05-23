using System.ComponentModel.DataAnnotations.Schema;

namespace Sup.Mm.Common.Entities;

public class Tag
{
    [Column("id")]
    public long Id { get; set; }
    
    [Column("name")]
    public string Name { get; set; } = string.Empty;
}