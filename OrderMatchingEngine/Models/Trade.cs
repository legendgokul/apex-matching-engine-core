using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderMatchingEngine.Models
{
    internal class Trade
    {
        public long buyid { get; set; }
        public long sellid { get; set; }
        public long quantity { get; set; }
        public long price_limit { get; set; }
        public DateTime createdon { get; set; }

    }
}
