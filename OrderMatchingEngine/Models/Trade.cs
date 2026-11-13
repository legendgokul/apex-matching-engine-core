using OrderMatchingEngine.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderMatchingEngine.Models
{
    internal class Trade
    {
        public long TakerOrderId { get; set; }
        public long MakerOrderId { get; set; }
        public long quantity { get; set; }
        public long price { get; set; } // allways depend on Maker/ resting order value.
        public OrderSide Side { get; set; }
        public DateTime createdon { get; set; }

    }
}
