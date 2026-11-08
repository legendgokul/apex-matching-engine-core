using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderMatchingEngine.Models
{
    public abstract record EngineEvent;

    public record OrderAccepted(long OrderId) : EngineEvent;

    public record TradeExecuted(long BuyOrderId, long SellOrderId, long Price, long Quantity) : EngineEvent;

    public record OrderRejected(long OrderId, string Message) : EngineEvent;

    public record OrderCancelled(long OrderId, long RemainingQuantity) : EngineEvent;

    public record InvalidRequest(long OrderId, string Message) : EngineEvent;

}
