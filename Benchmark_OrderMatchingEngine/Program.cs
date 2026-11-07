using BenchmarkDotNet.Attributes;
using OrderMatchingEngine.Models;
using OrderMatchingEngine.Enums;
using BenchmarkDotNet.Running;

class OrderMatchingEngineBenchmark
{
    public static void Main(String[] args)
    {
        var result = BenchmarkRunner.Run<TestOrderBook>();
    }

}

[MemoryDiagnoser]
public class TestOrderBook
{
    OrderBook _order = null!;

    [Params(1000, 100000, 10000000)]
    public int orderCount { get; set; }

    [Params(1000,100)]
    public int PriceLevelCount { get; set; }


    [IterationSetup]
    public void Setup()
    {
        _order = new OrderBook();
    }


    [Benchmark]
    public void TestCreateOrder()
    {
        for (int i = 1; i < orderCount; i++)
        {
            var order = new OrderInfo(
                i,
                200 + (i % PriceLevelCount),
                5,
                orderType.buy);

            _order.CreateOrder(order);
        }
    }
}