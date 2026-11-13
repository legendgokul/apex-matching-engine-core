using OrderMatchingEngine.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderMatchingEngine.Models
{
    internal readonly record struct CurrentSnapshot
        (List<restingOrder> BuyBids, List<restingOrder> SellBids, restingOrder[] Lookups);

    internal readonly record struct restingOrder
        (long Id, long Price, long Quantity, OrderSide Side);
}
