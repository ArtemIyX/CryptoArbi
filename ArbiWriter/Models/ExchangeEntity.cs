using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ArbiWriter.Models
{
    [Table("Exchanges")]
    public class ExchangeEntity
    {
        [Key, Required, NotNull]
        public required string Id { get; set; }

        [Required, NotNull]
        public required string Name { get; set; }

        [MaybeNull, DefaultValue(null)]
        public string? Url { get; set; } = null;

        [Required, NotNull, DefaultValue(true)]
        public required bool Working { get; set; } = true;

        public virtual ICollection<ExchangeToken>? Tokens { get; set; }
    }
}
