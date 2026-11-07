using System;
using System.Collections.Generic;
using System.Text;
using OrderMatchingEngine.Enums;
using OrderMatchingEngine.Models;

namespace OrderMatchingEngine_test
{
    internal class OrderBook_TestData : TheoryData<OrderInfo>
    {
        public OrderBook_TestData()
        {
            Add(new OrderInfo(1, 100, 5, orderType.buy));
            Add(new OrderInfo(2, 101, 20, orderType.buy));
            Add(new OrderInfo(3, 99, 21, orderType.buy));
            Add(new OrderInfo(4, 101, 50, orderType.buy));
            Add(new OrderInfo(5, 105, 15, orderType.buy));
            Add(new OrderInfo(6, 100, 25, orderType.buy));

        }
    }
}
