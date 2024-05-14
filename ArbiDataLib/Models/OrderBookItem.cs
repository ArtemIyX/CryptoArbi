using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbiDataLib.Models
{
    [Table("OrderBooks")]
    public class OrderBookItem
    {
        [Key]
        public long Id { get; set; }

        [DefaultValue(false)]
        public bool IsAsk { get; set; }

        [DefaultValue(0.0)]
        public double Price { get; set; }

        [DefaultValue(0.0)]
        public double Volume { get; set; }

        public long ExchangeTokenId { get; set; }
        public virtual ExchangeToken? ExchangeToken { get; set; }
    }
}
