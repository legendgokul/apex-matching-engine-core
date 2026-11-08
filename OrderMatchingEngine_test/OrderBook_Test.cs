using OrderMatchingEngine.Enums;
using OrderMatchingEngine.Models;

namespace orderMatchingEngine_test;

public class OrderBook_Test
{
    private readonly OrderBook _orderBook = new();

    #region CreateOrder
    [Fact]
    public void CreateOrder_BuyOrderRests() 
    {
        // arrange 
        var order = new OrderInfo(_id: 1, _price: 100, _quant: 20, _side: orderType.buy);
        var ExpectedResponse = new List<EngineEvent>() { new OrderAccepted(order.id) };
        
        // Act
        var response  = _orderBook.CreateOrder(order, DateTime.Now);
        
        //Assert
        Assert.Equal(response, ExpectedResponse);
    }
    [Fact]
    public void CreateOrder_SellOrderRests() 
    {
        // arrange 
        var order = new OrderInfo(_id: 1, _price: 100, _quant: 20, _side: orderType.sell);
        var ExpectedResponse = new List<EngineEvent>() { new OrderAccepted(order.id) };

        // Act
        var response = _orderBook.CreateOrder(order, DateTime.Now);

        //Assert
        Assert.Equal(response, ExpectedResponse);
    }
    [Fact]
    public void CreateOrder_ExactMatch() 
    {
        // arrange 
        var sellOrder = new OrderInfo(_id: 1, _price: 100, _quant: 20, _side: orderType.sell);
        var buyOrder = new OrderInfo(_id: 2, _price: 100, _quant: 20, _side: orderType.buy);
        var ExpectedResponse = new List<EngineEvent>();
        ExpectedResponse.Add(new OrderAccepted(buyOrder.id));
        ExpectedResponse.Add(new TradeExecuted(buyOrder.id, sellOrder.id, buyOrder.price, buyOrder.quantity));

        // Act
        _orderBook.CreateOrder(sellOrder, DateTime.Now);
        var response = _orderBook.CreateOrder(buyOrder, DateTime.Now);

        //Assert
        Assert.Equal(response, ExpectedResponse);
    }



    [Fact] 
    public void CreateOrder_BuyOrderWithRemaining() 
    {
        var sellOrder = new OrderInfo(_id: 1, _price: 100, _quant: 20, _side: orderType.sell);
        var buyOrder = new OrderInfo(_id: 2, _price: 100, _quant: 30, _side: orderType.buy);
        var validateSellOrder = new OrderInfo(_id: 3, _price: 100, _quant: 10, _side: orderType.sell);

        var expectedResult1 = new List<EngineEvent>();
        expectedResult1.Add(new OrderAccepted(buyOrder.id));
        expectedResult1.Add(new TradeExecuted(buyOrder.id, sellOrder.id, sellOrder.price, sellOrder.quantity));
        var time = DateTime.Now;

        var expectedResult2 = new List<EngineEvent>();
        expectedResult2.Add(new OrderAccepted(validateSellOrder.id));
        expectedResult2.Add(new TradeExecuted(buyOrder.id, validateSellOrder.id, validateSellOrder.price, validateSellOrder.quantity));

        // Act
        _orderBook.CreateOrder(sellOrder, time);
        var response1 = _orderBook.CreateOrder(buyOrder, time);
        var response2 = _orderBook.CreateOrder(validateSellOrder, time);

        // Assert
        Assert.Equal(response1, expectedResult1);
        Assert.Equal(response2, expectedResult2);
    }
    [Fact] 
    public void CreateOrder_SellOrderWithRemaining() 
    {  
        var buyOrder = new OrderInfo(_id: 2, _price: 100, _quant: 30, _side: orderType.buy);
        var sellOrder = new OrderInfo(_id: 1, _price: 100, _quant: 40, _side: orderType.sell);
        var validateBuyOrder = new OrderInfo(_id: 3, _price: 100, _quant: 10, _side: orderType.buy);

        var expectedResult1 = new List<EngineEvent>();
        expectedResult1.Add(new OrderAccepted(sellOrder.id));
        expectedResult1.Add(new TradeExecuted(buyOrder.id, sellOrder.id, buyOrder.price, buyOrder.quantity));
        var time = DateTime.Now;

        var expectedResult2 = new List<EngineEvent>();
        expectedResult2.Add(new OrderAccepted(validateBuyOrder.id));
        expectedResult2.Add(new TradeExecuted(validateBuyOrder.id, sellOrder.id, validateBuyOrder.price, validateBuyOrder.quantity));

        // Act
        _orderBook.CreateOrder( buyOrder, time);
        var response1 = _orderBook.CreateOrder(sellOrder, time);
        var response2 = _orderBook.CreateOrder(validateBuyOrder, time);

        // Assert
        Assert.Equal(response1, expectedResult1);
        Assert.Equal(response2, expectedResult2);
    }



    // verify cleanup once order is mapped.
    [Fact]
    public void CreateOrder_VerifySellOrderCleanUp()
    {
        // arrange 
        var sellOrder = new OrderInfo(_id: 1, _price: 100, _quant: 20, _side: orderType.sell);
        var buyOrder = new OrderInfo(_id: 2, _price: 100, _quant: 20, _side: orderType.buy);
        
        var ExpectedResponse = new List<EngineEvent>();
        ExpectedResponse.Add(new OrderAccepted(buyOrder.id));
        ExpectedResponse.Add(new TradeExecuted(buyOrder.id, sellOrder.id, buyOrder.price, buyOrder.quantity));

        var ExpectedResponse2 = new List<EngineEvent>();
        ExpectedResponse2.Add(new OrderAccepted(sellOrder.id));


        // Act
        _orderBook.CreateOrder(sellOrder, DateTime.Now);
        var response = _orderBook.CreateOrder(buyOrder, DateTime.Now);
        var response2 = _orderBook.CreateOrder(sellOrder, DateTime.Now);

        //Assert
        Assert.Equal(response, ExpectedResponse);
        Assert.Equal(response2, ExpectedResponse2);
    }
    [Fact]
    public void CreateOrder_VerifyBuyOrderCleanUp()
    {
        // arrange 
        var buyOrder = new OrderInfo(_id: 2, _price: 100, _quant: 20, _side: orderType.buy);
        var sellOrder = new OrderInfo(_id: 1, _price: 100, _quant: 20, _side: orderType.sell);

        var ExpectedResponse = new List<EngineEvent>();
        ExpectedResponse.Add(new OrderAccepted(sellOrder.id));
        ExpectedResponse.Add(new TradeExecuted(buyOrder.id, sellOrder.id, buyOrder.price, buyOrder.quantity));

        var ExpectedResponse2 = new List<EngineEvent>();
        ExpectedResponse2.Add(new OrderAccepted(buyOrder.id));


        // Act
        _orderBook.CreateOrder(buyOrder, DateTime.Now);
        var response = _orderBook.CreateOrder(sellOrder, DateTime.Now);
        var response2 = _orderBook.CreateOrder(buyOrder, DateTime.Now);

        //Assert
        Assert.Equal(response, ExpectedResponse);
        Assert.Equal(response2, ExpectedResponse2);
    }



    // price priority
    [Fact]
    public void CreateOrder_PricePriority_BuyOrder()
    {
        var restingOrder1 = new OrderInfo(_id: 1, _price: 100, _quant: 30, _side: orderType.buy);
        var restingOrder2 = new OrderInfo(_id: 2, _price: 110, _quant: 20, _side: orderType.buy);
        var restingOrder3 = new OrderInfo(_id: 3, _price: 110, _quant: 20, _side: orderType.buy);
        var SellOrder1 = new OrderInfo(_id: 4, _price: 90, _quant: 30, _side: orderType.sell);
        var SellOrder2 = new OrderInfo(_id: 5, _price: 95, _quant: 40, _side: orderType.sell);

        var expectedResult = new List<EngineEvent>();
        expectedResult.Add(new OrderAccepted(SellOrder1.id));
        expectedResult.Add(new TradeExecuted(restingOrder3.id, SellOrder1.id, restingOrder3.price, restingOrder3.quantity));
        expectedResult.Add(new TradeExecuted(restingOrder2.id, SellOrder1.id, restingOrder2.price, 10));

        var expectedResult2 = new List<EngineEvent>();
        expectedResult2.Add(new OrderAccepted(SellOrder2.id));
        expectedResult2.Add(new TradeExecuted(restingOrder2.id, SellOrder2.id, restingOrder2.price, 10));
        expectedResult2.Add(new TradeExecuted(restingOrder1.id, SellOrder2.id, restingOrder1.price, restingOrder1.quantity));
        var time = DateTime.Now;

        // ACT
        _orderBook.CreateOrder(restingOrder1, time);
        _orderBook.CreateOrder(restingOrder3, time);
        _orderBook.CreateOrder(restingOrder2, time);
        var Response1 = _orderBook.CreateOrder(SellOrder1, time);
        var Response2 = _orderBook.CreateOrder(SellOrder2, time);

        //Assert
        Assert.Equal(Response1, expectedResult); 
        Assert.Equal(Response2, expectedResult2);
    }
    [Fact]
    public void CreateOrder_PricePriority_SellOrder()
    {
        var restingOrder1 = new OrderInfo(_id: 1, _price: 100, _quant: 30, _side: orderType.sell);
        var restingOrder2 = new OrderInfo(_id: 2, _price: 110, _quant: 20, _side: orderType.sell);
        var restingOrder3 = new OrderInfo(_id: 3, _price: 110, _quant: 20, _side: orderType.sell);
        var BuyOrder1 = new OrderInfo(_id: 4, _price: 120, _quant: 40, _side: orderType.buy);
        var BuyOrder2 = new OrderInfo(_id: 5, _price: 110, _quant: 30, _side: orderType.buy);

        var expectedResult = new List<EngineEvent>();
        expectedResult.Add(new OrderAccepted(BuyOrder1.id));
        expectedResult.Add(new TradeExecuted(BuyOrder1.id, restingOrder1.id, restingOrder1.price, restingOrder1.quantity));
        expectedResult.Add(new TradeExecuted(BuyOrder1.id, restingOrder3.id, restingOrder3.price, 10));

        var expectedResult2 = new List<EngineEvent>();
        expectedResult2.Add(new OrderAccepted(BuyOrder2.id));
        expectedResult2.Add(new TradeExecuted(BuyOrder2.id, restingOrder3.id, restingOrder3.price, 10));
        expectedResult2.Add(new TradeExecuted(BuyOrder2.id, restingOrder2.id, restingOrder2.price, restingOrder2.quantity));
        var time = DateTime.Now;

        // ACT
        _orderBook.CreateOrder(restingOrder1, time);
        _orderBook.CreateOrder(restingOrder3, time);
        _orderBook.CreateOrder(restingOrder2, time);
        var Response1 = _orderBook.CreateOrder(BuyOrder1, time);
        var Response2 = _orderBook.CreateOrder(BuyOrder2, time);

        //Assert
        Assert.Equal(Response1, expectedResult);
        Assert.Equal(Response2, expectedResult2);
    }



    // FIFO priority
    [Fact] 
    public void CreateOrder_FIFOforBuyOrder()
    {
        var restingOrder1 = new OrderInfo(_id: 1, _price: 100, _quant: 30, _side: orderType.buy);
        var restingOrder2 = new OrderInfo(_id: 2, _price: 110, _quant: 20, _side: orderType.buy);
        var restingOrder3 = new OrderInfo(_id: 3, _price: 110, _quant: 20, _side: orderType.buy);
        var SellOrder = new OrderInfo(_id: 4, _price: 90, _quant: 70, _side: orderType.sell);


        var expectedResult = new List<EngineEvent>();
        expectedResult.Add(new OrderAccepted(SellOrder.id));
        expectedResult.Add(new TradeExecuted(restingOrder3.id, SellOrder.id, restingOrder3.price, restingOrder3.quantity));
        expectedResult.Add(new TradeExecuted(restingOrder2.id, SellOrder.id, restingOrder2.price, restingOrder2.quantity)); // order 2 comes later
        expectedResult.Add(new TradeExecuted(restingOrder1.id, SellOrder.id, restingOrder1.price, restingOrder1.quantity));

        var time = DateTime.Now;

        // Act
        _orderBook.CreateOrder(restingOrder1, time);
        _orderBook.CreateOrder(restingOrder3, time);
        _orderBook.CreateOrder(restingOrder2, time);
        var Response = _orderBook.CreateOrder(SellOrder, time);

        // Assert
        Assert.Equal(Response, expectedResult);
    }

    [Fact]
    public void CreateOrder_FIFOforSellOrder()
    {
        // Arrange
        var restingOrder1 = new OrderInfo(_id: 1, _price: 100, _quant: 30, _side: orderType.sell);
        var restingOrder2 = new OrderInfo(_id: 2, _price: 110, _quant: 20, _side: orderType.sell);
        var restingOrder3 = new OrderInfo(_id: 3, _price: 110, _quant: 20, _side: orderType.sell);
        var BuyOrder = new OrderInfo(_id: 4, _price: 120, _quant: 70, _side: orderType.buy);

        var expectedResult = new List<EngineEvent>();
        expectedResult.Add(new OrderAccepted(BuyOrder.id));
        expectedResult.Add(new TradeExecuted(BuyOrder.id, restingOrder1.id, restingOrder1.price, restingOrder1.quantity));
        expectedResult.Add(new TradeExecuted(BuyOrder.id, restingOrder3.id, restingOrder3.price, restingOrder3.quantity));
        expectedResult.Add(new TradeExecuted(BuyOrder.id, restingOrder2.id, restingOrder2.price, restingOrder2.quantity)); // order 2 comes later

        var time = DateTime.Now;

        // Act
        _orderBook.CreateOrder(restingOrder1, time);
        _orderBook.CreateOrder(restingOrder3, time);
        _orderBook.CreateOrder(restingOrder2, time);
        var Response = _orderBook.CreateOrder(BuyOrder, time);

        // Assert
        Assert.Equal(Response, expectedResult);
    }



    // Validation Failure 
    public static IEnumerable<object[]> Validation_OrderId_TestData => new List<object[]>
    {
        new object[] { 2,new List<EngineEvent>() { new OrderAccepted(2) } },
        new object[] { 0, new List<EngineEvent>() { new OrderRejected(0, "Invalid Order ID") } },
        new object[] { -1,new List<EngineEvent>() { new OrderRejected(-1, "Invalid Order ID") } },
        new object[] { 1, new List<EngineEvent>() { new OrderRejected(1, "Order Already Exists") } }
    };

    [Theory]
    [MemberData(nameof(Validation_OrderId_TestData))]
    public void CreateOrder_ValidateForOrderID(int orderid, List<EngineEvent> expectedEvent)
    {
        //arrange
        var restingOrder = new OrderInfo(1, 100, 10, orderType.buy);
        var order = new OrderInfo(orderid, 100, 10, orderType.buy);
        var datatime = DateTime.Now;

        var restingOrderResponse = new List<EngineEvent>() { new OrderAccepted(1) };
        // ACT
        var response1 = _orderBook.CreateOrder(restingOrder, datatime);
        var response2 = _orderBook.CreateOrder(order, datatime);

        //Assert
        Assert.Equal(response1, restingOrderResponse);
        Assert.Equal(response2, expectedEvent);
    }



    public static IEnumerable<object[]> Validation_OrderType_TestData => new List<object[]> 
            { 
                new object[] { 1, orderType.buy, new List<EngineEvent>() { new OrderAccepted(1) } },
                new object[] { 1, orderType.sell, new List<EngineEvent>() { new OrderAccepted(1) } },
                new object[] { 1, (orderType)99, new List<EngineEvent>() { new OrderRejected(1, "Invalid order type") } }
            };

    [Theory]
    [MemberData(nameof(Validation_OrderType_TestData))]
    public void CreateOrder_ValidateForOrderType(long id, orderType type, List<EngineEvent> expectedResponse)
    {
        var order = new OrderInfo(id, 100, 10, type);
        var datetime = DateTime.Now;

        var response = _orderBook.CreateOrder(order, datetime);

        Assert.Equal(response, expectedResponse);
    }



    public static IEnumerable<object[]> Validation_Price_TestData => new List<object[]>
            {
                new object[] { 1, 100, new List<EngineEvent>() { new OrderAccepted(1) } },
                new object[] { 1, -100, new List<EngineEvent>() { new OrderRejected(1, "Invalid Price") } },
                new object[] { 1, 0, new List<EngineEvent>() { new OrderRejected(1, "Invalid Price") } }
            };

    [Theory]
    [MemberData(nameof(Validation_Price_TestData))]
    public void CreateOrder_ValidateForPrice(long id, long price, List<EngineEvent> expectedResponse)
    {
        var order = new OrderInfo(id, price, 10, orderType.buy);
        var datetime = DateTime.Now;

        var response = _orderBook.CreateOrder(order, datetime);

        Assert.Equal(response, expectedResponse);
    }



    public static IEnumerable<object[]> Validation_quantity_TestData => new List<object[]>
            {
                new object[] { 1, 100, new List<EngineEvent>() { new OrderAccepted(1) } },
                new object[] { 1, -100, new List<EngineEvent>() { new OrderRejected(1, "Invalid Quantity") } },
                new object[] { 1, 0, new List<EngineEvent>() { new OrderRejected(1, "Invalid Quantity") } }
            };

    [Theory]
    [MemberData(nameof(Validation_quantity_TestData))]
    public void CreateOrder_ValidateForQuantity(long id, long quantity, List<EngineEvent> expectedResponse)
    {
        var order = new OrderInfo(id, 10, quantity, orderType.buy);
        var datetime = DateTime.Now;

        var response = _orderBook.CreateOrder(order, datetime);

        Assert.Equal(response, expectedResponse);
    }
    #endregion


    #region CancelOrder

    [Fact]
    public void CancelOrder_restingBuyOrder()
    {
        // Arrange
        var order = new OrderInfo(_id: 1, _price: 100, _quant: 20, _side: orderType.buy);
        var datetime = DateTime.Now;
        var expectedResponse = new List<EngineEvent>() { new OrderAccepted(1) };
        var cancelledResponse =  new OrderCancelled(1, 20) ;
        
        // Act
        var response = _orderBook.CreateOrder(order, datetime);
        var cancelResponse = _orderBook.CancelOrder(order.id);

        // Assert
        Assert.Equal(response, expectedResponse);
        Assert.Equal(cancelResponse, cancelledResponse);

    }

    [Fact]
    public void CancelOrder_restingSellOrder()
    {
        // Arrange
        var order = new OrderInfo(_id: 1, _price: 100, _quant: 20, _side: orderType.sell);
        var datetime = DateTime.Now;
        var expectedResponse = new List<EngineEvent>() { new OrderAccepted(1) };
        var cancelledResponse = new OrderCancelled(1, 20);

        // Act
        var response = _orderBook.CreateOrder(order, datetime);
        var cancelResponse = _orderBook.CancelOrder(order.id);

        // Assert
        Assert.Equal(response, expectedResponse);
        Assert.Equal(cancelResponse, cancelledResponse);
    }



    public static IEnumerable<object[]> CancelOrder_InvalidOrderId_TestData => new List<object[]>()
    {
        new object[] { 1, new OrderCancelled(1, 20) },
        new object[] { -15, new InvalidRequest(-15, "Invalid orderid") },
        new object[] { 0, new InvalidRequest(0, "Invalid orderid")},
    };

    [Theory]
    [MemberData(nameof(CancelOrder_InvalidOrderId_TestData))]
    public void CancelOrder_InvalidOrderId(long id, EngineEvent eventResponse)
    {
        // Arrange
        var order = new OrderInfo(_id: 1, _price: 100, _quant: 20, _side: orderType.buy);
        var datetime = DateTime.Now;
        var expectedResponse = new List<EngineEvent>() { new OrderAccepted(1) };
        var cancelledResponse = new OrderCancelled(1, 20);

        // Act
        var response = _orderBook.CreateOrder(order, datetime);
        var cancelResponse = _orderBook.CancelOrder(id);

        // Assert
        Assert.Equal(response, expectedResponse);
        Assert.Equal(cancelResponse, eventResponse);
    }



    public static IEnumerable<object[]> CancelOrder_MissingOrder_TestData => new List<object[]>()
    {
        new object[] { 1, new OrderCancelled( 1, 20) },
        new object[] { 2, new InvalidRequest( 2, "Order not found") }
    };

    [Theory]
    [MemberData(nameof(CancelOrder_MissingOrder_TestData))]
    public void CancelOrder_MissingOrder(long id, EngineEvent eventResponse)
    {
        // Arrange
        var order = new OrderInfo(_id: 1, _price: 100, _quant: 20, _side: orderType.buy);
        var datetime = DateTime.Now;

        var expectedResponse = new List<EngineEvent>() { new OrderAccepted(1) };
        var cancelledResponse = new OrderCancelled(1, 20);

        // Act
        var response = _orderBook.CreateOrder(order, datetime);
        var cancelResponse = _orderBook.CancelOrder(id);

        // Assert
        Assert.Equal(response, expectedResponse);
        Assert.Equal(cancelResponse, eventResponse);
    }




    #endregion

}
