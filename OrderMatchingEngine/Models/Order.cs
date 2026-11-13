using OrderMatchingEngine.Enums;
using System.Runtime.CompilerServices;

namespace OrderMatchingEngine.Models
{
    public enum RequestType
    {
        CreateOrder, CancelOrder
    }

    // This is the object that gets stored in ledger
    internal struct OrderInfo
    {
        public long Id { get; set; } // orderid
        public long Price { get; set; }
        public long Quantity { get; set; }
        public long RemainingQuantity { get; set; }
        public OrderSide Side { get; set; }
    }

    public struct OrderRequest
    {
        // cancel 
        public OrderRequest(long CID, long id, long time)
        {
            CommandId = CID;
            RequestType = RequestType.CancelOrder;
            Id = id;
            CurrentDateTime = time;
        }

        // create
        public OrderRequest(long CID, long id,long price, long quant, long time, OrderSide side )
        {
            CommandId = CID;
            RequestType = RequestType.CreateOrder;

            Id = id;
            Price = price;
            Quantity = quant;
            RemainingQuantity = quant; 
            CurrentDateTime = time;
            Side = side;
        }

        public long CommandId { get; set; }
        public RequestType RequestType { get; set; }

        public long Id { get; set; } // orderid
        public long Price { get; set; }
        public long Quantity { get; set; }
        public long RemainingQuantity { get; set; }
        public long CurrentDateTime { get; set; }
        public OrderSide Side { get; set; }
    }

}
