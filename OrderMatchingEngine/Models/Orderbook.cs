using OrderMatchingEngine.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

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
        public bool CreateOrder(OrderInfo order)
        {
            // validation to reject the order. 
            if (order.quantity <= 0) return false;

            if (order.price <= 0) return false;

            if (order.id <= 0)
                return false;

            if (orderLookup.ContainsKey(order.id))
            {
                return false;
            }

            if(order.Side == orderType.buy)
            {
                AddToBuyledger(order.id, order.quantity, order.price);
            }
            else if (order.Side == orderType.sell)
            {
                AddToSellledger(order.id, order.quantity, order.price);
            }
            else
            {
                return false; //invalid enum
            }

            return true;
        }

        private bool AddToBuyledger(long order_id, long quant, long price)
        {

            if (!buyLedger.TryGetValue(price, out var level))
            {
                level = new PriceLevel();
                buyLedger[price] = level;
            }

            var data = new OrderInfo();
            data.id = order_id;
            data.quantity = quant;
            data.remainingQuantity = quant;
            data.Side = orderType.buy;
            data.price = price;

            var TailNode = level.Tail;
            var newNode = new LinkedListNode(data);
            TailNode.Next = newNode;
            newNode.Prev = TailNode;

            // update Tail tracker
            level.Tail = newNode;

            orderLookup[order_id] = newNode;
            return true;
        }

        // first node will always be default 
        private bool AddToSellledger(long order_id, long quant, long price)
        {

            if(!sellLedger.TryGetValue(price, out var level))
            {
                level = new PriceLevel();
                sellLedger[price] = level;
            }

            var data = new OrderInfo();
            data.id = order_id;
            data.quantity = quant;
            data.remainingQuantity = quant;
            data.Side = orderType.sell;
            data.price = price;

            var TailNode = level.Tail;
            var newNode = new LinkedListNode(data);
            TailNode.Next = newNode;
            newNode.Prev = TailNode;

            // update Tail tracker
            level.Tail = newNode;

            orderLookup[order_id] = newNode;
            return true;
        }

        public bool CancelOrder(long order_id)
        {
            if (order_id <= 0)
                return false;

            if (!orderLookup.TryGetValue(order_id, out var order))
                return false;

            var price = order.Data.price;
            var side = order.Data.Side;


            // Remove empty ledger entries 
            var ledger = side == orderType.buy ? buyLedger : sellLedger;

            if (!ledger.TryGetValue(price, out var level))
                return false; // Defensive consistency check


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

            // Remove from lookup
            orderLookup.Remove(order_id);

            if (level.Head .Next == null)
            {
                ledger.Remove(price);
            }

            return true;
        }
    }
}
