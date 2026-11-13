using OrderMatchingEngine.Enums;
using OrderMatchingEngine.Models;

namespace orderMatchingEngine_test;

public static class FakeClock
{
    public static long GetCurrentTicks() => 639301859506289491;
}

public class OrderBook_Test
{
    private readonly OrderBook _orderBook = new();

    [Fact]
    public void CreateOrder_BuyOrderRests()
    {
        // arrange 
        var order =
            new OrderRequest(1, 1, 100, 20, FakeClock.GetCurrentTicks(), OrderSide.buy);

        var ExpectedResponse = new List<EngineEvent>() {
            new EngineEvent(1,1,1,OrderSide.buy,order.CurrentDateTime)
        };


        // Act
        var response = _orderBook.ProcessOrder(order);

        //Assert
        Assert.True(response.SequenceEqual(ExpectedResponse), "THey are not same!");
    }

    [Fact]
    public void CreateOrder_SellOrderRests()
    {
        // arrange 
        var order = new OrderRequest(1, 1, 100, 20, FakeClock.GetCurrentTicks(), OrderSide.sell);

        var ExpectedResponse = new List<EngineEvent>() {
            new EngineEvent(1,1,1,OrderSide.sell,order.CurrentDateTime)
        };

        // Act
        var response = _orderBook.ProcessOrder(order);

        //Assert
        Assert.Equal(ExpectedResponse, response);
    }

    [Fact]
    public void CreateOrder_ExactMatch()
    {

        // arrange 
        var sellOrder = new OrderRequest(1, 1, 100, 20, FakeClock.GetCurrentTicks(), OrderSide.sell);
        var buyOrder = new OrderRequest(2, 2, 100, 20, FakeClock.GetCurrentTicks(), OrderSide.buy);

        var ExpectedResponse = new List<EngineEvent>();
        ExpectedResponse.Add(
            new EngineEvent(2, buyOrder.CommandId, buyOrder.Id, OrderSide.buy, buyOrder.CurrentDateTime));
        ExpectedResponse.Add(
            new EngineEvent(3, buyOrder.CommandId, buyOrder.Id, sellOrder.Id, sellOrder.Price,
                                sellOrder.Quantity, OrderSide.buy, buyOrder.CurrentDateTime)
            );

        // Act
        var snapshot1 = _orderBook.SnapShot();
        _orderBook.ProcessOrder(sellOrder);
        var response = _orderBook.ProcessOrder(buyOrder);
        var snapshot2 = _orderBook.SnapShot();

        //Assert
        Assert.Equal(ExpectedResponse, response);
        Assert.Equal(snapshot1.Lookups, snapshot2.Lookups);
        Assert.Equal(snapshot1.BuyBids, snapshot2.BuyBids);
        Assert.Equal(snapshot1.SellBids, snapshot2.SellBids);
    }

    [Fact]
    public void CreateOrder_BuyOrderWithRemaining()
    {
        var Order1 = new OrderRequest(1, 1, 100, 20, FakeClock.GetCurrentTicks(), OrderSide.sell);

        var Order2 = new OrderRequest(2, 2, 100, 30, FakeClock.GetCurrentTicks(), OrderSide.buy);

        var Order3 = new OrderRequest(3, 3, 100, 10, FakeClock.GetCurrentTicks(), OrderSide.sell);



        var expectedResult1 = new List<EngineEvent>();
        expectedResult1.Add(
            new EngineEvent(2, Order2.CommandId, Order2.Id, Order2.Side, Order2.CurrentDateTime));
        expectedResult1.Add(
            new EngineEvent(3, Order2.CommandId, Order2.Id, Order1.Id, Order1.Price,
                                Order1.Quantity, Order2.Side, Order2.CurrentDateTime)
            );


        var expectedResult2 = new List<EngineEvent>();
        expectedResult2.Add(
            new EngineEvent(4, Order3.CommandId, Order3.Id, Order3.Side, Order3.CurrentDateTime));
        expectedResult2.Add(
            new EngineEvent(5, Order3.CommandId, Order3.Id, Order2.Id, Order2.Price,
                                Order3.Quantity, Order3.Side, Order3.CurrentDateTime)
            );

        // Act
        _orderBook.ProcessOrder(Order1);
        var response1 = _orderBook.ProcessOrder(Order2);
        var response2 = _orderBook.ProcessOrder(Order3);
        var snapshot2 = _orderBook.SnapShot();

        // Assert
        Assert.Equal(expectedResult1, response1);
        Assert.Equal(expectedResult2, response2);
        Assert.Empty(snapshot2.Lookups);
        Assert.Empty(snapshot2.BuyBids);
        Assert.Empty(snapshot2.SellBids);

    }

    [Fact]
    public void CreateOrder_SellOrderWithRemaining()
    {

        var Order1 = new OrderRequest(1, 1, 100, 20, FakeClock.GetCurrentTicks(), OrderSide.buy);
        var Order2 = new OrderRequest(2, 2, 100, 30, FakeClock.GetCurrentTicks(), OrderSide.sell);
        var Order3 = new OrderRequest(3, 3, 100, 10, FakeClock.GetCurrentTicks(), OrderSide.buy);


        var expectedResult1 = new List<EngineEvent>();
        expectedResult1.Add(
            new EngineEvent(2, Order2.CommandId, Order2.Id, Order2.Side, Order2.CurrentDateTime));
        expectedResult1.Add(
            new EngineEvent(3, Order2.CommandId, Order2.Id, Order1.Id, Order1.Price,
                                Order1.Quantity, Order2.Side, Order2.CurrentDateTime)
            );


        var expectedResult2 = new List<EngineEvent>();
        expectedResult2.Add(
            new EngineEvent(4, Order3.CommandId, Order3.Id, Order3.Side, Order3.CurrentDateTime));
        expectedResult2.Add(
            new EngineEvent(5, Order3.CommandId, Order3.Id, Order2.Id, Order2.Price,
                                Order3.Quantity, Order3.Side, Order3.CurrentDateTime)
            );


        // Act
        _orderBook.ProcessOrder(Order1);
        var response1 = _orderBook.ProcessOrder(Order2);
        var response2 = _orderBook.ProcessOrder(Order3);
        var snapshot2 = _orderBook.SnapShot();

        // Assert
        Assert.Equal(expectedResult1, response1);
        Assert.Equal(expectedResult2, response2);
        Assert.Empty(snapshot2.Lookups);
        Assert.Empty(snapshot2.BuyBids);
        Assert.Empty(snapshot2.SellBids);
    }

    // price priority
    [Fact]
    public void CreateOrder_PricePriority_BuyOrder()
    {
        var Order1 =
            new OrderRequest(1, 1, 100, 30, FakeClock.GetCurrentTicks(), OrderSide.buy);
        var Order2 =
            new OrderRequest(2, 2, 110, 20, FakeClock.GetCurrentTicks(), OrderSide.buy);
        var Order3 =
            new OrderRequest(3, 3, 110, 20, FakeClock.GetCurrentTicks(), OrderSide.buy);
        var Order4 =
            new OrderRequest(4, 4, 90, 30, FakeClock.GetCurrentTicks(), OrderSide.sell);
        var Order5 =
             new OrderRequest(5, 5, 95, 40, FakeClock.GetCurrentTicks(), OrderSide.sell);


        var expectedResult = new List<EngineEvent>();
        expectedResult.Add(
            new EngineEvent(4, Order4.CommandId, Order4.Id, Order4.Side, Order4.CurrentDateTime));
        expectedResult.Add(
            new EngineEvent(5, Order4.CommandId, Order4.Id, Order3.Id, Order3.Price, Order3.Quantity, Order4.Side, Order4.CurrentDateTime));
        expectedResult.Add(
            new EngineEvent(6, Order4.CommandId, Order4.Id, Order2.Id, Order2.Price, 10, Order4.Side, Order4.CurrentDateTime));

        var expectedResult2 = new List<EngineEvent>();
        expectedResult2.Add(
            new EngineEvent(7, Order5.CommandId, Order5.Id, Order5.Side, Order5.CurrentDateTime));
        expectedResult2.Add(
            new EngineEvent(8, Order5.CommandId, Order5.Id, Order2.Id, Order2.Price, 10, Order5.Side, Order5.CurrentDateTime));
        expectedResult2.Add(
            new EngineEvent(9, Order5.CommandId, Order5.Id, Order1.Id, Order1.Price, Order1.Quantity, Order5.Side, Order5.CurrentDateTime));


        // ACT
        _orderBook.ProcessOrder(Order1);
        _orderBook.ProcessOrder(Order3);
        _orderBook.ProcessOrder(Order2);
        var Response1 = _orderBook.ProcessOrder(Order4);
        var Response2 = _orderBook.ProcessOrder(Order5);
        var snapShot = _orderBook.SnapShot();


        //Assert
        Assert.Equal(expectedResult, Response1);
        Assert.Equal(expectedResult2, Response2);
        Assert.Empty(snapShot.Lookups);  // to verify if the internal state is clean
        Assert.Empty(snapShot.BuyBids);
        Assert.Empty(snapShot.SellBids);
    }

    [Fact]
    public void CreateOrder_PricePriority_SellOrder()
    {

        var Order1 =
            new OrderRequest(1, 1, 100, 30, FakeClock.GetCurrentTicks(), OrderSide.sell);
        var Order2 =
            new OrderRequest(2, 2, 110, 20, FakeClock.GetCurrentTicks(), OrderSide.sell);
        var Order3 =
            new OrderRequest(3, 3, 110, 20, FakeClock.GetCurrentTicks(), OrderSide.sell);
        var Order4 =
            new OrderRequest(4, 4, 120, 40, FakeClock.GetCurrentTicks(), OrderSide.buy);
        var Order5 =
             new OrderRequest(5, 5, 110, 30, FakeClock.GetCurrentTicks(), OrderSide.buy);


        var expectedResult = new List<EngineEvent>();
        expectedResult.Add(
            new EngineEvent(4, Order4.CommandId, Order4.Id, Order4.Side, Order4.CurrentDateTime));
        expectedResult.Add(
            new EngineEvent(5, Order4.CommandId, Order4.Id, Order1.Id, Order1.Price, Order1.Quantity, Order4.Side, Order4.CurrentDateTime));
        expectedResult.Add(
            new EngineEvent(6, Order4.CommandId, Order4.Id, Order3.Id, Order3.Price, 10, Order4.Side, Order4.CurrentDateTime));

        var expectedResult2 = new List<EngineEvent>();
        expectedResult2.Add(
            new EngineEvent(7, Order5.CommandId, Order5.Id, Order5.Side, Order5.CurrentDateTime));
        expectedResult2.Add(
            new EngineEvent(8, Order5.CommandId, Order5.Id, Order3.Id, Order3.Price, 10, Order5.Side, Order5.CurrentDateTime));
        expectedResult2.Add(
            new EngineEvent(9, Order5.CommandId, Order5.Id, Order2.Id, Order2.Price, Order2.Quantity, Order5.Side, Order5.CurrentDateTime));


        // ACT
        _orderBook.ProcessOrder(Order1);
        _orderBook.ProcessOrder(Order3);
        _orderBook.ProcessOrder(Order2);
        var Response1 = _orderBook.ProcessOrder(Order4);
        var Response2 = _orderBook.ProcessOrder(Order5);
        var snapShot = _orderBook.SnapShot();

        //Assert
        Assert.Equal(expectedResult, Response1);
        Assert.Equal(expectedResult2, Response2);
        Assert.Empty(snapShot.Lookups);  // to verify if the internal state is clean
        Assert.Empty(snapShot.BuyBids);
        Assert.Empty(snapShot.SellBids);
    }

    // Validation Failure 
    public static IEnumerable<object[]> Validation_OrderId_TestData => new List<object[]>
      {
          new object[] { 2,new List<EngineEvent>() { new EngineEvent(2, 2, 2, OrderSide.buy, FakeClock.GetCurrentTicks() ) }  },
          new object[] { -1,new List<EngineEvent>() { new EngineEvent(2, 2, -1, RejectReason.InvalidOrderId, FakeClock.GetCurrentTicks()) } },
          new object[] { 1,new List<EngineEvent>() { new EngineEvent(2, 2, 1, RejectReason.DuplicateOrderId, FakeClock.GetCurrentTicks())}  },
          new object[] { 0,new List<EngineEvent>() { new EngineEvent(2, 2, 0, RejectReason.InvalidOrderId, FakeClock.GetCurrentTicks())} },
      };

    [Theory]
    [MemberData(nameof(Validation_OrderId_TestData))]
    public void CreateOrder_ValidateForOrderID(int orderid, List<EngineEvent> expectedEvent)
    {
        //arrange
        var restingOrder = new OrderRequest(1, 1, 100, 10, FakeClock.GetCurrentTicks(), OrderSide.buy);

        var order = new OrderRequest(2, orderid, 100, 10, FakeClock.GetCurrentTicks(), OrderSide.buy);

        // ACT
        _orderBook.ProcessOrder(restingOrder);
        var response2 = _orderBook.ProcessOrder(order);

        //Assert
        Assert.Equal(expectedEvent, response2);
    }



    public static IEnumerable<object[]> Validation_OrderType_TestData => new List<object[]>
             {
                 new object[] { 1,
                     OrderSide.buy,
                     new List<EngineEvent>() { new EngineEvent(1, 1, 1, OrderSide.buy, FakeClock.GetCurrentTicks() ) }
                 },
                 new object[] { 1,
                     OrderSide.sell,
                     new List<EngineEvent>() { new EngineEvent(1, 1, 1, OrderSide.sell, FakeClock.GetCurrentTicks() ) }
                 },
                 new object[] { 1,
                     (OrderSide)99,
                     new List<EngineEvent>() { new EngineEvent(1, 1, 1, RejectReason.InvalidSide, FakeClock.GetCurrentTicks()) }
                 }
             };

    [Theory]
    [MemberData(nameof(Validation_OrderType_TestData))]
    public void CreateOrder_ValidateForOrderType(long id, OrderSide type, List<EngineEvent> expectedResponse)
    {
        var order = new OrderRequest(1, id, 100, 10, FakeClock.GetCurrentTicks(), type);

        var response = _orderBook.ProcessOrder(order);

        Assert.Equal(response, expectedResponse);
    }



    public static IEnumerable<object[]> Validation_Price_TestData => new List<object[]>
          {
              new object[] { 1, 100,
                  new List<EngineEvent>() { new EngineEvent(1, 1, 1, OrderSide.buy, FakeClock.GetCurrentTicks() ) }
              },
              new object[] { 1, -100,
                  new List<EngineEvent>() { new EngineEvent(1, 1, 1, RejectReason.InvalidPrice, FakeClock.GetCurrentTicks()) }
              },
              new object[] { 1, 0,
                 new List<EngineEvent>() { new EngineEvent(1, 1, 1, RejectReason.InvalidPrice, FakeClock.GetCurrentTicks()) }
              }
          };

    [Theory]
    [MemberData(nameof(Validation_Price_TestData))]
    public void CreateOrder_ValidateForPrice(long id, long price, List<EngineEvent> expectedResponse)
    {
        var order = new OrderRequest(1, id, price, 10, FakeClock.GetCurrentTicks(), OrderSide.buy);

        var response = _orderBook.ProcessOrder(order);

        Assert.Equal(response, expectedResponse);
    }



    public static IEnumerable<object[]> Validation_quantity_TestData => new List<object[]>
          {
              new object[] { 1, 100,
                  new List<EngineEvent>() { new EngineEvent(1, 1, 1, OrderSide.buy, FakeClock.GetCurrentTicks() ) }
              },
              new object[] { 1, -100,
                  new List<EngineEvent>() { new EngineEvent(1, 1, 1, RejectReason.InvalidQuantity, FakeClock.GetCurrentTicks()) }
              },
              new object[] { 1, 0,
                 new List<EngineEvent>() { new EngineEvent(1, 1, 1, RejectReason.InvalidQuantity, FakeClock.GetCurrentTicks()) }
              }
          };

    [Theory]
    [MemberData(nameof(Validation_quantity_TestData))]
    public void CreateOrder_ValidateForQuantity(long id, long quantity, List<EngineEvent> expectedResponse)
    {
        var order = new OrderRequest(1, id, 10, quantity, FakeClock.GetCurrentTicks(), OrderSide.buy);

        var response = _orderBook.ProcessOrder(order);

        Assert.Equal(response, expectedResponse);
    }



    #region CancelOrder

    [Fact]
    public void CancelOrder_restingBuyOrder()
    {
        // Arrange
        var order = new OrderRequest(1, 1, 10, 10, FakeClock.GetCurrentTicks(), OrderSide.buy);
        var cancelledResponse = new List<EngineEvent>() { new EngineEvent(2, 2, 1, 10, OrderSide.buy, FakeClock.GetCurrentTicks()) };

        // Act
        _orderBook.ProcessOrder(order);
        var cancelResponse = _orderBook.ProcessOrder(new OrderRequest(2, 1, FakeClock.GetCurrentTicks()));

        // Assert
        Assert.Equal(cancelResponse, cancelledResponse);
    }

    [Fact]
    public void CancelOrder_restingSellOrder()
    {
        // Arrange
        var order = new OrderRequest(1, 1, 10, 10, FakeClock.GetCurrentTicks(), OrderSide.sell);
        var ExpectedResponse = new List<EngineEvent>() { new EngineEvent(2, 2, 1, 10, OrderSide.sell, FakeClock.GetCurrentTicks()) };

        // Act
        _orderBook.ProcessOrder(order);
        var CancelResponse = _orderBook.ProcessOrder(new OrderRequest(2, 1, FakeClock.GetCurrentTicks()));

        // Assert
        Assert.Equal(ExpectedResponse, CancelResponse);
    }



    public static IEnumerable<object[]> CancelOrder_InvalidOrderId_TestData => new List<object[]>()
    {
      new object[] { 1,
          new List<EngineEvent>(){ new EngineEvent(2,2,1,20,OrderSide.buy,FakeClock.GetCurrentTicks()) }
      },
      new object[] { -15,
          new List<EngineEvent>() { new EngineEvent(2, 2, -15, RejectReason.InvalidOrderId, FakeClock.GetCurrentTicks()) }
      },
      new object[] { 0,
          new List<EngineEvent>() { new EngineEvent(2, 2, 0, RejectReason.InvalidOrderId, FakeClock.GetCurrentTicks()) }
      },
    };

    [Theory]
    [MemberData(nameof(CancelOrder_InvalidOrderId_TestData))]
    public void CancelOrder_InvalidOrderId(long id, List<EngineEvent> eventResponse)
    {
        // Arrange
        var order = new OrderRequest(1, 1, 100, 20, FakeClock.GetCurrentTicks(), OrderSide.buy);

        // Act
        _orderBook.ProcessOrder(order);
        var cancelResponse = _orderBook.ProcessOrder(new OrderRequest(2, id, FakeClock.GetCurrentTicks()));

        // Assert
        Assert.Equal(eventResponse, cancelResponse);
    }



    public static IEnumerable<object[]> CancelOrder_MissingOrder_TestData => new List<object[]>()
    {
        new object[] { 1,
            new List<EngineEvent>(){ new EngineEvent(2,2,1,10,OrderSide.buy,FakeClock.GetCurrentTicks()) }
        },
        new object[] { 2,
            new List<EngineEvent>() { new EngineEvent(2, 2, 2, RejectReason.OrderNotFound, FakeClock.GetCurrentTicks()) }
        }
    };

    [Theory]
    [MemberData(nameof(CancelOrder_MissingOrder_TestData))]
    public void CancelOrder_MissingOrder(long id, List<EngineEvent> eventResponse)
    {
        // Arrange
        var order = new OrderRequest(1,1,10,10,FakeClock.GetCurrentTicks(),OrderSide.buy);

        // Act
         _orderBook.ProcessOrder(order);
        var cancelResponse = _orderBook.ProcessOrder(new OrderRequest(2, id, FakeClock.GetCurrentTicks()));

        // Assert
        Assert.Equal(eventResponse, cancelResponse);
    }

    #endregion

}
