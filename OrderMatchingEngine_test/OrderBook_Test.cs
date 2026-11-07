using OrderMatchingEngine.Enums;
using OrderMatchingEngine.Models;
using OrderMatchingEngine_test;

namespace orderMatchingEngine_test;

public class OrderBook_Test
{
    private readonly OrderBook _orderBook = new();

    #region CreateOrder 
    [Fact]
    public void CreateOrder_ValidBuyOrder_ReturnsTrue()
    {
        // arrange 
        var order = new OrderInfo(1, 200, 20, orderType.buy);

        // ACT
        var result = _orderBook.CreateOrder(order);

        //ASSERT
        Assert.True(result);

    }

    [Fact]
    public void CreateOrder_ValidSellOrder_ReturnsTrue()
    {
        // arrange 
        var order = new OrderInfo(2, 200, 20, orderType.sell);

        // ACT
        var result = _orderBook.CreateOrder(order);

        //ASSERT
        Assert.True(result);

    }

    
    [Fact]
    public void CreateOrder_ValidateDuplicate_ReturnsFalse()
    {
        // arrange 
        var first = new OrderInfo(1, 200, 20, orderType.sell);
        var duplciate = new OrderInfo(1, 200, 20, orderType.sell);

        // act
        var result = _orderBook.CreateOrder(first);
        var result2 = _orderBook.CreateOrder(duplciate);

        // assert
        Assert.True(result);
        Assert.False(result2);
    }


    [Theory]
    [InlineData(0, 100, 5)]
    [InlineData(-1, 100, 5)]
    [InlineData(1, 0, 5)]
    [InlineData(1, -1, 5)]
    [InlineData(1, 10, -5)]
    [InlineData(1, 10, 0)]
    public void CreateOrder_InvalidOrders_ReturnsFalse(long id, long price, long quant)
    {
        // arrange 
        var order = new OrderInfo(id, price, quant, orderType.buy);

        // act
        var result = _orderBook.CreateOrder(order);

        // assert
        Assert.False(result);

    } 

    [Fact]
    public void CreateOrder_ExistingOrder_ReturnFalse()
    {
        // arrange
        var order = new OrderInfo(1, 100, 2, orderType.sell);

        //act
        var result1 = _orderBook.CreateOrder(order);
        var result2 = _orderBook.CreateOrder(order);


        // assert
        Assert.True(result1);
        Assert.False(result2);
    }

    #endregion


    #region CancelOrder
    [Fact]
    public void CancelOrder_ValidCancelHeadNode_ReturnTrue()
    {
        //arrange
        var first = new OrderInfo(1,100, 2, orderType.buy);
        var second = new OrderInfo(2,100, 3, orderType.buy);

        //ACt
        _orderBook.CreateOrder(first);
        _orderBook.CreateOrder(second);
        var result = _orderBook.CancelOrder(first.id);
        // try recreating
        var recreate = _orderBook.CreateOrder(first);


        // asset
        Assert.True(result);
        Assert.True(recreate);
    }


    [Fact]
    public void CancelOrder_ValidCancelTailNode_ReturnTrue()
    {
        //arrange
        var first = new OrderInfo(1, 100, 2, orderType.buy);
        var second = new OrderInfo(2, 100, 3, orderType.buy);

        //ACt
        _orderBook.CreateOrder(first);
        _orderBook.CreateOrder(second);
        var result = _orderBook.CancelOrder(second.id);

        // try recreating
        var recreate = _orderBook.CreateOrder(second);

        // asset
        Assert.True(result);
        Assert.True(recreate);
    }

    [Fact]
    public void CancelOrder_ValidCancelMidNode_ReturnTrue()
    {
        //arrange
        var first = new OrderInfo(1, 100, 2, orderType.buy);
        var second = new OrderInfo(2, 100, 3, orderType.buy);
        var third = new OrderInfo(3, 100, 3, orderType.buy);

        //ACt
        _orderBook.CreateOrder(first);
        _orderBook.CreateOrder(second);
        _orderBook.CreateOrder(third);
        var result = _orderBook.CancelOrder(second.id);

        // try recreating
        var recreate = _orderBook.CreateOrder(second);

        // asset
        Assert.True(result);
        Assert.True(recreate);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-1)]
    public void CancelOrder_InvalidCancel_ReturnFalse(int order_id)
    {

        //ACt
        var result = _orderBook.CancelOrder(order_id);

        // asset
        Assert.False(result);
    }

    #endregion
}
