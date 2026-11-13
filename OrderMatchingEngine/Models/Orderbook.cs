using OrderMatchingEngine.Enums;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;

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
        public LinkedListNode Head { get; set; }
        public LinkedListNode Tail { get; set; }

        public PriceLevel()
        {
            Head = new LinkedListNode();
            Tail = Head;
        }
    }

    internal class OrderBook
    {
        private long _nextEventSeq = 1;
        private long NextSeq() => _nextEventSeq++;

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

        public List<EngineEvent> ProcessOrder(OrderRequest order)
        {
            if (order.RequestType == RequestType.CreateOrder)
            {
                return CreateOrder(order);
            }
            else
            {
                return new List<EngineEvent>() { CancelOrder(order) };
            }
        }

        // methods to add unit to ledger
        private List<EngineEvent> CreateOrder(OrderRequest orderRequest)
        {
            var EventList = new List<EngineEvent>();

            // validation to reject the order. 

            if (orderRequest.Id <= 0)
            {
                EventList.Add(new EngineEvent(NextSeq(), orderRequest.CommandId, orderRequest.Id,
                                        RejectReason.InvalidOrderId, orderRequest.CurrentDateTime));
                return EventList;
            }

            if (orderLookup.ContainsKey(orderRequest.Id))
            {
                EventList.Add(new EngineEvent(NextSeq(), orderRequest.CommandId, orderRequest.Id,
                                        RejectReason.DuplicateOrderId, orderRequest.CurrentDateTime));
                return EventList;
            }

            if (orderRequest.Quantity <= 0)
            {
                EventList.Add(new EngineEvent(NextSeq(), orderRequest.CommandId, orderRequest.Id,
                                        RejectReason.InvalidQuantity, orderRequest.CurrentDateTime));
                return EventList;
            }

            if (orderRequest.Price <= 0)
            {
                EventList.Add(new EngineEvent(NextSeq(), orderRequest.CommandId, orderRequest.Id,
                                        RejectReason.InvalidPrice, orderRequest.CurrentDateTime));
                return EventList;
            }

            if (orderRequest.Side != OrderSide.buy && orderRequest.Side != OrderSide.sell)
            {
                EventList.Add(new EngineEvent(NextSeq(), orderRequest.CommandId, orderRequest.Id,
                                        RejectReason.InvalidSide, orderRequest.CurrentDateTime));
                return EventList;
            }

            EventList.Add(
                new EngineEvent(NextSeq(), orderRequest.CommandId, orderRequest.Id, orderRequest.Side, orderRequest.CurrentDateTime));


            EventList.AddRange(MatchBooking(ref orderRequest));

            if (orderRequest.RemainingQuantity > 0)
            {
                if (orderRequest.Side == OrderSide.buy)
                {
                    AddToBuyledger(orderRequest);
                }
                else if (orderRequest.Side == OrderSide.sell)
                {
                    AddToSellledger(orderRequest);
                }
            }

            return EventList;
        }

        private EngineEvent CancelOrder(OrderRequest orderRequest)
        {

            if (orderRequest.Id <= 0)
            {
                return new EngineEvent(NextSeq(), orderRequest.CommandId, orderRequest.Id,
                                        RejectReason.InvalidOrderId, orderRequest.CurrentDateTime);

            }

            if (!orderLookup.TryGetValue(orderRequest.Id, out var order))
            {
                return new EngineEvent(NextSeq(), orderRequest.CommandId, orderRequest.Id,
                                        RejectReason.OrderNotFound, orderRequest.CurrentDateTime);
            }

            UnlinkNode(order);

            return new EngineEvent(NextSeq(), orderRequest.CommandId, orderRequest.Id, order.Data.RemainingQuantity,
                                        order.Data.Side, orderRequest.CurrentDateTime);
        }

        private List<EngineEvent> MatchBooking(ref OrderRequest orderRequest)
        {
            var oppositeLedger = (orderRequest.Side == OrderSide.sell) ? buyLedger : sellLedger;
            var result = new List<EngineEvent>();

            while (orderRequest.RemainingQuantity > 0 && oppositeLedger.Count > 0)
            {
                // pick the first price
                var FirstItem = oppositeLedger.First();

                // check if we can use this price
                bool canUse = (orderRequest.Side == OrderSide.buy) ? (orderRequest.Price >= FirstItem.Key) : (orderRequest.Price <= FirstItem.Key);

                if (!canUse)
                {
                    break;
                }

                var restingOrder = FirstItem.Value.Head.Next;

                if (restingOrder != null)
                {
                    var resultingQuant = Math.Min(orderRequest.RemainingQuantity, restingOrder.Data.RemainingQuantity);

                    orderRequest.RemainingQuantity -= resultingQuant;
                    restingOrder.Data.RemainingQuantity -= resultingQuant;


                    result.Add(
                        new EngineEvent(NextSeq(), orderRequest.CommandId, orderRequest.Id, restingOrder.Data.Id,
                        FirstItem.Key, resultingQuant, orderRequest.Side, orderRequest.CurrentDateTime)
                    );

                    if (restingOrder.Data.RemainingQuantity < 1)
                    {
                        UnlinkNode(restingOrder);
                    }
                }
                else
                {
                    break;
                }
            }
            return result;
        }

        private void AddToBuyledger(OrderRequest orderRequest)
        {
            if (!buyLedger.TryGetValue(orderRequest.Price, out var level))
            {
                level = new PriceLevel();
                buyLedger[orderRequest.Price] = level;
            }

            var TailNode = level.Tail;
            var newNode = new LinkedListNode(new OrderInfo()
            {
                Id = orderRequest.Id,
                Price = orderRequest.Price,
                RemainingQuantity = orderRequest.RemainingQuantity,
                Quantity = orderRequest.Quantity,
                Side = orderRequest.Side
            });

            TailNode.Next = newNode;
            newNode.Prev = TailNode;

            // update Tail tracker
            level.Tail = newNode;

            orderLookup[orderRequest.Id] = newNode;

        }

        private void AddToSellledger(OrderRequest orderRequest)
        {
            if (!sellLedger.TryGetValue(orderRequest.Price, out var level))
            {
                level = new PriceLevel();
                sellLedger[orderRequest.Price] = level;
            }

            var TailNode = level.Tail;
            var newNode = new LinkedListNode(new OrderInfo()
            {
                Id = orderRequest.Id,
                Price = orderRequest.Price,
                RemainingQuantity = orderRequest.RemainingQuantity,
                Quantity = orderRequest.Quantity,
                Side = orderRequest.Side
            });

            TailNode.Next = newNode;
            newNode.Prev = TailNode;

            // update Tail tracker
            level.Tail = newNode;

            orderLookup[orderRequest.Id] = newNode;
        }

        private void UnlinkNode(LinkedListNode order)
        {
            var price = order.Data.Price;
            var Id = order.Data.Id;

            // Remove empty ledger entries 
            var ledger = (order.Data.Side == OrderSide.buy)
                ? buyLedger
                : sellLedger;

            if (!ledger.TryGetValue(price, out var level))
                throw new InvalidOperationException("Order book is inconsistent."); // Defensive consistency check

            // Unlink previous node
            if (order.Prev != null)
                order.Prev.Next = order.Next;

            // Unlink next node 
            if (order.Next != null)
                order.Next.Prev = order.Prev;

            // check if we are removing Tail
            if (level.Tail == order)
            {
                level.Tail = order.Prev;
            }

            // Detach deleted node
            order.Prev = null;
            order.Next = null;

            // Remove empty price level
            if (level.Head.Next == null)
            {
                ledger.Remove(price);
            }

            // Remove from lookup
            orderLookup.Remove(Id);
        }

        internal CurrentSnapshot SnapShot()
        {
            var lookup = new restingOrder[orderLookup.Count];
            int index = 0;
            foreach (var item in orderLookup.Values)
            {
                lookup[index++] = new restingOrder(
                        item.Data.Id, item.Data.Price, item.Data.RemainingQuantity, item.Data.Side
                        );
            }

            // Dictionary Order can be unpredictable with future change so sorting it for safety
            Array.Sort(lookup, Comparer<restingOrder>.Create((a, b) => a.Id.CompareTo(b.Id)));

            var buyerSide = new List<restingOrder>();
            index = 0;
            foreach (var item in buyLedger.Values)
            {
                var head = item.Head.Next; //skipping starting demmy node

                while (head != null)
                {
                    buyerSide.Add(new restingOrder(head.Data.Id,
                        head.Data.Price,
                        head.Data.RemainingQuantity,
                        head.Data.Side));

                    head = head.Next;
                }
            }

            var sellerSide = new List<restingOrder>();
            index = 0;
            foreach (var item in sellLedger.Values)
            {
                var head = item.Head.Next; //skipping starting demmy node

                while (head != null)
                {
                    sellerSide.Add(new restingOrder(head.Data.Id,
                        head.Data.Price,
                        head.Data.RemainingQuantity,
                        head.Data.Side));

                    head = head.Next;
                }
            }

            return new CurrentSnapshot(buyerSide, sellerSide, lookup);
        }

    }
}
