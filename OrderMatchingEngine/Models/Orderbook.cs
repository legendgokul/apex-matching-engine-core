using OrderMatchingEngine.Enums;

//limit order book
namespace OrderMatchingEngine.Models
{
    internal class LinkedListNode
    {
        public OrderInfo Data;
        public LinkedListNode? Next;
        public LinkedListNode? Prev;

        public LinkedListNode(OrderInfo order = new OrderInfo())
        {
            Data = order;
        }
    }

    internal class PriceLevel
    {
        public LinkedListNode Head  { get; set; }
        public LinkedListNode Tail { get; set; }

        public PriceLevel()
        {
            Head  = new LinkedListNode();
            Tail = Head ;
        }
    }

    internal class OrderBook
    {
        private SortedDictionary<long, PriceLevel> buyLedger;
        private SortedDictionary<long, PriceLevel> sellLedger;
        private Dictionary<long, LinkedListNode> orderLookup;

        public OrderBook()
        {
            sellLedger = new();
            buyLedger = new(
                 Comparer<long>.Create((a, b) => b.CompareTo(a))
                 );
            orderLookup = new();

        }

        // methods to add unit to ledger
        public List<EngineEvent> CreateOrder(OrderInfo order, DateTime currentTime)
        {
            var EventList = new List<EngineEvent>();

            // validation to reject the order. 
            if (order.quantity <= 0)
                return new List<EngineEvent>() { new OrderRejected(order.id, "Invalid Quantity") };
            
            if (order.price <= 0)
                return new List<EngineEvent>() { new OrderRejected(order.id, "Invalid Price") };

            if (order.id <= 0)
                return new List<EngineEvent>() { new OrderRejected(order.id, "Invalid Order ID") };

            if (orderLookup.ContainsKey(order.id))
                return new List<EngineEvent>() { new OrderRejected(order.id, "Order Already Exists") };

            if( order.Side != orderType.buy && order.Side != orderType.sell)
                return new List<EngineEvent>() { new OrderRejected(order.id, "Invalid order type") };

            EventList.Add(new OrderAccepted(order.id));

            var Trades = MatchBooking(ref order, currentTime);

            foreach (var t in Trades)
            {
                EventList.Add(new TradeExecuted(t.buyid, t.sellid, t.price, t.quantity));
            }

            if (order.remainingQuantity > 0)
            {
                if (order.Side == orderType.buy)
                {        
                    AddToBuyledger(order);
                }
                else if (order.Side == orderType.sell)
                {
                    AddToSellledger(order);
                }
            }
           
            return EventList;
        }

        public EngineEvent CancelOrder(long id)
        {
            if (id <= 0)
                return new InvalidRequest(id, "Invalid orderid");

            if (!orderLookup.TryGetValue(id, out var order))
                return new InvalidRequest(id, "Order not found");

            return RemoveOrder(order);
        }

        private List<Trade> MatchBooking(ref OrderInfo order, DateTime currentTime)
        {
            var oppositeLedger = (order.Side == orderType.sell)? buyLedger : sellLedger;
            var result = new List<Trade>();


            while(order.remainingQuantity > 0 && oppositeLedger.Count > 0)
            {
                // pick the first price
                var price = oppositeLedger.First().Key;

                // check if we can use this price
                bool canUse = (order.Side == orderType.buy) ? (order.price >= price) : (order.price <= price);

                if(!canUse)
                {
                    break;
                }

                if(!oppositeLedger.TryGetValue(price, out var level))
                {
                    break; 
                }

                var restingOrder = level.Head.Next;

                if(restingOrder != null)
                { 
                    var resultingQuant = Math.Min(order.remainingQuantity, restingOrder.Data.remainingQuantity);

                    order.remainingQuantity -= resultingQuant;
                    restingOrder.Data.remainingQuantity -= resultingQuant;

                    var trade = new Trade();
                    trade.buyid = (order.Side == orderType.buy) ? order.id : restingOrder.Data.id;
                    trade.sellid = (order.Side == orderType.sell) ? order.id : restingOrder.Data.id;
                    trade.price = price;
                    trade.quantity = resultingQuant;
                    trade.createdon = currentTime;

                    result.Add(trade);

                    if(restingOrder.Data.remainingQuantity < 1)
                    {
                        RemoveOrder(restingOrder);
                    }
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        private void AddToBuyledger(OrderInfo data)
        {
            if (!buyLedger.TryGetValue(data.price, out var level))
            {
                level = new PriceLevel();
                buyLedger[data.price] = level;
            }

            var TailNode = level.Tail;
            var newNode = new LinkedListNode(data);
            TailNode.Next = newNode;
            newNode.Prev = TailNode;

            // update Tail tracker
            level.Tail = newNode;

            orderLookup[data.id] = newNode;

        }

        // first node will always be default 
        private void AddToSellledger(OrderInfo data)
        { 
            if(!sellLedger.TryGetValue(data.price, out var level))
            {
                level = new PriceLevel();
                sellLedger[data.price] = level;
            }

            var TailNode = level.Tail;
            var newNode = new LinkedListNode(data);
            TailNode.Next = newNode;
            newNode.Prev = TailNode;

            // update Tail tracker
            level.Tail = newNode;

            orderLookup[data.id] = newNode;
        }

        private EngineEvent RemoveOrder(LinkedListNode order)
        {   
            var price = order.Data.price;
            var id = order.Data.id;
            var remaining = order.Data.remainingQuantity;

            // Remove empty ledger entries 
            var ledger = (order.Data.Side == orderType.buy) ? buyLedger : sellLedger;

            if (!ledger.TryGetValue(price, out var level))
                return new InvalidRequest(id,"order not found"); // Defensive consistency check


            // Unlink previous node
            if (order.Prev != null)
                order.Prev.Next = order.Next;

            // Unlink next node (if it exists)
            if (order.Next != null)
                order.Next.Prev = order.Prev;

            if (level.Tail == order) // checking if we are removing Tail
            {
                level.Tail = order.Prev;
            }

            // Detach deleted node
            order.Prev = null;
            order.Next = null;

            if (level.Head.Next == null)
            {
                ledger.Remove(price);
            }

            // Remove from lookup
            orderLookup.Remove(id);

            return new OrderCancelled(id, remaining);
        }

    }
}
