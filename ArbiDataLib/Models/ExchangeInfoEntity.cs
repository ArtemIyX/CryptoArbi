using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbiDataLib.Models
{
    public class ExchangeInfoEntity
    {
        public string ExchangeId { get; set; } = string.Empty;
        public string HomeUrl { get; set; } = string.Empty;
        public string TradeUrl { get; set; } = string.Empty;
        public string DepositUrl { get; set; } = string.Empty;
        public string WithdrawalUrl { get; set;} = string.Empty;
    }
}
