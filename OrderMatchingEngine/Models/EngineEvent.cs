using OrderMatchingEngine.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace OrderMatchingEngine.Models
{
    /*
     * EvenSeq : used to sequentially order event stream in WAL ( created by engine )
     * commandId : unique id for each request. ( provided by caller ? )
     */

    public readonly record struct EngineEvent
    {

        // Reject
        public EngineEvent(long seq, long cid, long TOid, RejectReason reason, long time)
        {
            Kind = EventType.OrderRejected;
            EventSequence = seq;
            CommandId = cid;
            TakerOrderId = TOid;
            Reason = reason;
            CreatedOn = time;
        }

        // Order Accepted
        public EngineEvent(long seq, long cid, long TOid, OrderSide side, long time)
        {
            Kind = EventType.OrderAccepted;
            EventSequence = seq;
            CommandId = cid;
            TakerOrderId = TOid;
            Takerside = side;
            CreatedOn = time;
        }

        // OrderCancel
        public EngineEvent(long seq, long cid, long id, long quant, OrderSide side, long time)
        {
            Kind = EventType.OrderCancelled;
            EventSequence = seq;
            CommandId = cid;
            TakerOrderId = id;
            Takerside = side;
            RemainingQuantity = quant;
            CreatedOn = time;
        }

        // Trade Executed
        public EngineEvent(long seq, long cid, long TOid,long MOid, long price, long quant, OrderSide side, long time)
        {
            Kind = EventType.TradeExecuted;
            EventSequence = seq;
            CommandId = cid;
            TakerOrderId = TOid;
            MakerOrderId = MOid;
            Price = price;
            quantity = quant;
            Takerside = side;
            CreatedOn = time;    
        }

        

        public EventType Kind { get; init; }
        public long EventSequence { get; init; }
        public long CommandId { get; init; }

        public long TakerOrderId { get; init; }
        public long MakerOrderId { get; init; }
        public long Price { get; init; } // Depends on MAKER order
        public long quantity { get; init; }
        public OrderSide Takerside { get; init; }

        public long RemainingQuantity { get; init; }
        public RejectReason Reason { get; init; }

        // when the event was generated.
        public long CreatedOn { get; init; }

        // for down stream systems 
        public long BuyerOrderId => (Takerside == OrderSide.buy) ? TakerOrderId : MakerOrderId;
        public long SellerOrderId => (Takerside == OrderSide.buy) ? MakerOrderId : TakerOrderId;
    }

}
