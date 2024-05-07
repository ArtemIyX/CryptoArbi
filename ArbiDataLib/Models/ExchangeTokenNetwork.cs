using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace ArbiDataLib.Models
{
    [Table("Networks")]
    public class ExchangeTokenNetwork
    {
        [Key]
        public long Id { get; set; }

        [NotNull, DefaultValue("Network")]
        public string Code { get; set; }

        [NotNull, DefaultValue(false)]
        public bool Active { get; set; }

        [NotNull, DefaultValue(false)]
        public bool Deposit { get; set; }

        [NotNull, DefaultValue(false)]
        public bool Withdraw { get; set; }

        public long ExchangeTokenId { get; set; }
        public virtual ExchangeToken Token { get; set; }
    }
}
