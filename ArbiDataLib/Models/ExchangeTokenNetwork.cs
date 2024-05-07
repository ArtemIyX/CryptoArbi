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

        [NotNull, DefaultValue(null)]
        public double? Fee { get; set; } = null;

        public long ExchangeTokenId { get; set; }
        public virtual ExchangeToken Token { get; set; }

        public TokenNetworkResponse ToResponse() => 
            new(Code, Active, Deposit, Withdraw, Fee);
    }

    public class TokenNetworkResponse
    {
        public string Code { get; set; } = string.Empty;

        public bool Active { get; set; } = false;

        public bool Deposit { get; set; } = false;

        public bool Withdraw { get; set; } = false;

        public double? Fee { get; set; } = null;

        // Empty Constructor
        public TokenNetworkResponse()
        {
            Code = string.Empty;
            Active = false;
            Deposit = false;
            Withdraw = false;
        }

        // Full Constructor
        public TokenNetworkResponse(string code, bool active, bool deposit, bool withdraw,
            double? fee)
        {
            Code = code;
            Active = active;
            Deposit = deposit;
            Withdraw = withdraw;
            Fee = fee;
        }
    }
}
