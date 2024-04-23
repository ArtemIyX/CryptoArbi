using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace ArbiDataLib.Models
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

        public ExchangeEntityResponse ToReposnse() => new ExchangeEntityResponse()
        {
            Id = Id,
            Name = Name,
            Url = Url,
            Working = Working,
        };

        public long InfoId { get; set; }
        public virtual ExchangeInfoEntity? Info { get; set; }
    }

    public class ExchangeEntityResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; } = null;

        [JsonPropertyName("working")]
        public bool Working { get; set; } = true;
    }
}
