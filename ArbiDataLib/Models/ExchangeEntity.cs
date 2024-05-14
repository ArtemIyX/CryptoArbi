using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using ArbiDataLib.Data;

namespace ArbiDataLib.Models
{
    public class ExchangeInfoData
    {
        public IList<ExchangeEntityResponse> Exchanges { get; set; } = [];
        public IList<ExchangeUrlInfo> ExchangeUrls { get; set; } = [];
    }

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

    }

    public class ExchangeEntityResponse
    {
        public ExchangeEntityResponse() { }
   
        public ExchangeEntityResponse(string id, string name, string? url, bool working)
        {
            Id = id;
            Name = name;
            Url = url;
            Working = working;
        }

        public ExchangeEntityResponse(ExchangeEntityResponse other) 
            : this(other.Id, other.Name, other.Url, other.Working) { }

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string? Url { get; set; } = null;

        [JsonPropertyName("working")]
        public bool Working { get; set; } = true;
    }

}
