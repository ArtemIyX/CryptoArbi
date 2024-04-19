using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ArbiWriter.Models
{
    [Table("Tokens")]
    public class ExchangeToken
    {
        [Key, Required, NotNull]
        public required int Id { get; set; }

        [Required, NotNull]
        public required string FullSymbolName { get; set; }

        [Required, NotNull]
        public required string DisplayName { get; set; }

        [MaybeNull, DefaultValue(null)]
        public double? Ask { get; set; }

        [MaybeNull, DefaultValue(null)]
        public double? Bid { get; set; }

        [MaybeNull, DefaultValue(null)]
        public double? DayVolumeUSDT { get; set; }

        [MaybeNull, DefaultValue(null)]
        public double? AskVolume { get; set; }

        [MaybeNull, DefaultValue(null)]
        public double? BidVolume { get; set; }

        public required string ExchangeId { get; set; }
        public virtual required ExchangeEntity Exchange { get; set; }

        public double? AskVolumeUsdt => AskVolume is not null ? AskVolume * Ask : null;
        public double? BidVolumeUsdt => AskVolume is not null ? BidVolume * Bid : null;
    }
}
