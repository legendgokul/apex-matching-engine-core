using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderMatchingEngine.Enums
{
    public enum EventType : byte
    {
        OrderAccepted, TradeExecuted, OrderCancelled, OrderRejected
    }
}
