using OrderMatchingEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderMatchingEngine.Enums
{
    public enum RejectReason : byte
    {
        InvalidQuantity = 1,
        InvalidPrice = 2,
        InvalidOrderId = 3,
        DuplicateOrderId = 4,
        InvalidSide = 5,
        OrderNotFound = 6
    }


}
